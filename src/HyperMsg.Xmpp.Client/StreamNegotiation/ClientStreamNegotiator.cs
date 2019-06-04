using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class ClientStreamNegotiator : IStreamNegotiator
    {
        private readonly Dictionary<string, IFeatureNegotiator> negotiators;

        public ClientStreamNegotiator(IEnumerable<IFeatureNegotiator> featureNegotiators)
        {
            negotiators = new Dictionary<string, IFeatureNegotiator>();

            foreach (var negotiator in featureNegotiators)
            {
                negotiators.Add(negotiator.FeatureName, negotiator);
            }
        }

        public async Task<StreamNegotiationResult> NegotiateAsync(ITransceiver<XmlElement, XmlElement> transceiver, XmppConnectionSettings settings, CancellationToken cancellationToken = default)
        {
            VerifySettings(settings);
            IEnumerable<XmlElement> features = null;
            var negotiatedFeatures = new Dictionary<string, FeatureNegotiationResult>();
            bool restartRequired = true;
            bool negotiating = true;

            while (negotiating)
            {
                if (restartRequired)
                {
                    await SendAndReceiveStreamHeaderAsync(transceiver, settings, cancellationToken);
                    features = await ReceiveFeaturesAsync(transceiver, cancellationToken);
                }

                if (negotiating = HasNegotiatorsForFeatures(features, negotiatedFeatures.Keys))
                {
                    var feature = SelectFeature(features, settings, negotiatedFeatures.Keys);
                    var negotiator = GetNegotiator(feature);
                    var result = await negotiator.NegotiateAsync(transceiver, feature);
                    negotiatedFeatures.Add(negotiator.FeatureName, result);
                    restartRequired = result.IsStreamRestartRequired;
                }
            }

            return new StreamNegotiationResult(negotiatedFeatures.Keys, negotiatedFeatures.Values);
        }

        private async Task SendAndReceiveStreamHeaderAsync(ITransceiver<XmlElement, XmlElement> transceiver, XmppConnectionSettings settings, CancellationToken cancellationToken)
        {
            var header = CreateHeader(settings.Domain);
            await transceiver.SendAsync(header, cancellationToken);
            var response = await transceiver.ReceiveNoStreamErrorAsync(cancellationToken);
            VerifyHeader(response);
        }

        private XmlElement CreateHeader(string domain)
        {
            return StreamHeader.Client().To(domain).NewId();
        }

        private void VerifyHeader(XmlElement header)
        {
            if (!IsStreamHeader(header))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:stream", header.Name));
            }
        }

        private bool IsStreamHeader(XmlElement element)
        {
            return element.Name == "stream:stream";
        }

        private async Task<IEnumerable<XmlElement>> ReceiveFeaturesAsync(ITransceiver<XmlElement, XmlElement> transceiver, CancellationToken cancellationToken)
        {
            var features = await transceiver.ReceiveNoStreamErrorAsync(cancellationToken);

            return GetFeatureItems(features);
        }

        private IEnumerable<XmlElement> GetFeatureItems(XmlElement features)
        {
            if (!IsFeatures(features))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:features", features.Name));
            }

            return features.Children;
        }

        private bool IsFeatures(XmlElement element)
        {
            return element.Name == "stream:features";
        }

        private IFeatureNegotiator GetNegotiator(XmlElement feature)
        {
            if (!negotiators.ContainsKey(feature.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.NoNegotiatorForFeature, feature.Name));
            }

            return negotiators[feature.Name];
        }

        private bool HasNegotiatorsForFeatures(IEnumerable<XmlElement> features, IEnumerable<string> negotiatedFeatures)
        {
            return features.Select(f => f.Name)
                .Except(negotiatedFeatures)
                .Any(f => negotiators.ContainsKey(f));
        }

        private XmlElement SelectFeature(IEnumerable<XmlElement> features, XmppConnectionSettings settings, IEnumerable<string> negotiatedFeatures)
        {
            if (HasTlsFeature(features)
                && settings.UseTls
                && !negotiatedFeatures.Contains("starttls")
                && negotiators.ContainsKey("starttls"))
            {
                return GetTlsFeature(features);
            }

            if (HasSaslFeature(features)
                && settings.UseSasl
                && !negotiatedFeatures.Contains("mechanisms")
                && negotiators.ContainsKey("mechanisms"))
            {
                return GetSaslFeature(features);
            }

            return features.FirstOrDefault(
                f => negotiators.ContainsKey(f.Name)
                && !negotiatedFeatures.Contains(f.Name));
        }

        private bool HasTlsFeature(IEnumerable<XmlElement> features)
        {
            return features.Any(f => f.Name == "starttls" && f.Xmlns() == XmppNamespaces.Tls);
        }

        private XmlElement GetTlsFeature(IEnumerable<XmlElement> features)
        {
            return features.First(f => f.Name == "starttls");
        }

        private bool HasSaslFeature(IEnumerable<XmlElement> features)
        {
            return features.Any(f => f.Name == "mechanisms" && f.Xmlns() == XmppNamespaces.Sasl);
        }

        private XmlElement GetSaslFeature(IEnumerable<XmlElement> features)
        {
            return features.First(f => f.Name == "mechanisms");
        }

        private void VerifySettings(XmppConnectionSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(settings.Domain))
            {
                throw new ArgumentException("Domain is empty.");
            }
        }
    }
}
