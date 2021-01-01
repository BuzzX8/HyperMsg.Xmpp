using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Extensions
{
    internal class ConnectTask : MessagingTask<bool>
    {
        private IFeatureNegotiator currentNegotiator;
        private StreamNegotiationState negotiationState;
        private List<IFeatureNegotiator> featureNegotiators;
        private List<XmlElement> negotiatedFeatures;
        private XmlElement currentFeature;
        private XmppConnectionSettings settings;

        public ConnectTask(IMessagingContext context, CancellationToken cancellationToken = default) : base(context, cancellationToken)
        {
            AddReceiver<XmlElement>(HandleAsync);
        }

        internal async Task<MessagingTask<bool>> StartAsync(XmppConnectionSettings connectionSettings)
        {
            featureNegotiators = new List<IFeatureNegotiator>(connectionSettings.FeatureNegotiators);
            negotiatedFeatures = new List<XmlElement>();
            settings = connectionSettings;
            var header = CreateHeader(connectionSettings.Domain);
            await TransmitAsync(header, CancellationToken);
            negotiationState = StreamNegotiationState.WaitingStreamHeader;

            return this;
        }

        private XmlElement CreateHeader(string domain) => StreamHeader.Client().To(domain).NewId();

        private Task HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            return negotiationState switch
            {
                StreamNegotiationState.WaitingStreamHeader => Task.FromResult(HandleStreamHeader(element)),
                StreamNegotiationState.WaitingStreamFeatures => HandleStreamFeaturesAsync(element, cancellationToken),
                StreamNegotiationState.NegotiatingFeature => HandleFeatureNegotiationMessageAsync(element, cancellationToken),
                _ => Task.CompletedTask
            };
        }

        private StreamNegotiationState HandleStreamHeader(XmlElement streamHeader)
        {
            VerifyStreamHeader(streamHeader);
            return negotiationState = StreamNegotiationState.WaitingStreamFeatures;
        }

        private void VerifyStreamHeader(XmlElement header)
        {
            if (!IsStreamHeader(header))
            {
                throw new XmppException($"InvalidXmlElementReceived. Expected stream:stream but received {header.Name}");
            }
        }

        private bool IsStreamHeader(XmlElement element) => element.Name == "stream:stream";

        private async Task HandleStreamFeaturesAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyFeaturesResponse(element);

            if (!element.HasChildren || !HasNegotiatorsForFeatures(element.Children))
            {
                negotiationState = StreamNegotiationState.Done;
                return;
            }

            var feature = SelectFeature(element.Children);
            currentNegotiator = GetNegotiator(feature);
            var isRestartRequired = await await currentNegotiator.NegotiateAsync(MessagingContext, feature, cancellationToken);
            currentFeature = feature;

            await HandleFeatureNegotiationStateAsync(isRestartRequired, cancellationToken);            
        }

        private void VerifyFeaturesResponse(XmlElement features)
        {
            if (!IsStreamFeatures(features))
            {
                throw new XmppException($"InvalidXmlElementReceived. Expected stream:features, but received {features.Name}");
            }
        }

        private async Task HandleFeatureNegotiationStateAsync(bool isRestartRequired, CancellationToken cancellationToken)
        {
            if (isRestartRequired)
            {
                var header = CreateHeader(settings.Domain);
                await TransmitAsync(header, cancellationToken);
                negotiationState = StreamNegotiationState.WaitingStreamHeader;
            }
            else
            {
                negotiationState = StreamNegotiationState.WaitingStreamFeatures;
                negotiatedFeatures.Add(currentFeature);
                currentFeature = null;
                currentNegotiator = null;
            }
        }

        private async Task HandleFeatureNegotiationMessageAsync(XmlElement message, CancellationToken cancellationToken)
        {
            var state = await await currentNegotiator.NegotiateAsync(MessagingContext, message, cancellationToken);

            await HandleFeatureNegotiationStateAsync(state, cancellationToken);
        }

        private bool IsStreamFeatures(XmlElement element) => element.Name == "stream:features";

        private bool HasNegotiatorsForFeatures(IEnumerable<XmlElement> features)
        {
            return features
                .Except(negotiatedFeatures)
                .Any(f => featureNegotiators.Any(c => c.CanNegotiate(f)));
        }

        private XmlElement SelectFeature(IEnumerable<XmlElement> features)
        {
            if (HasTlsFeature(features)
                && settings.UseTls
                && !negotiatedFeatures.Any(f => f.Name == "starttls"))
            {
                var tlsFeature = GetTlsFeature(features);

                if (HasNegotiatorForFeature(tlsFeature))
                {
                    return tlsFeature;
                }
            }

            if (HasSaslFeature(features)
                && settings.UseSasl
                && !negotiatedFeatures.Any(f => f.Name == "mechanisms"))
            {
                var saslFeature = GetSaslFeature(features);

                if (HasNegotiatorForFeature(saslFeature))
                {
                    return saslFeature;
                }
            }

            return features.FirstOrDefault(
                f => HasNegotiatorForFeature(f)
                && !negotiatedFeatures.Contains(f));
        }

        private bool HasTlsFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "starttls" && f.Xmlns() == XmppNamespaces.Tls);

        private XmlElement GetTlsFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "starttls");

        private bool HasSaslFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "mechanisms" && f.Xmlns() == XmppNamespaces.Sasl);

        private XmlElement GetSaslFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "mechanisms");

        private bool HasNegotiatorForFeature(XmlElement feature) => featureNegotiators.Any(c => c.CanNegotiate(feature));

        private IFeatureNegotiator GetNegotiator(XmlElement feature)
        {
            var component = featureNegotiators.FirstOrDefault(c => c.CanNegotiate(feature));

            if (component == null)
            {
                throw new InvalidOperationException($"NoNegotiatorForFeature {feature.Name}");
            }

            return component;
        }
    }
}
