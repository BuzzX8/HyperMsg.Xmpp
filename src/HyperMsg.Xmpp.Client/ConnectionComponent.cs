using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class ConnectionComponent
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly XmppConnectionSettings settings;

        public ConnectionComponent(IMessageSender<XmlElement> messageSender, XmppConnectionSettings settings)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings)); 
        }

        public IList<IFeatureComponent> FeatureComponents { get; } = new List<IFeatureComponent>();

        public StreamNegotiationState State { get; private set; }

        public async Task HandleTransportEventAsync(TransportEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var header = CreateHeader(settings.Domain);
            await messageSender.SendAsync(header, cancellationToken);
            State = StreamNegotiationState.WaitingStreamHeader;
        }

        public Task HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            switch (State)
            {
                case StreamNegotiationState.WaitingStreamHeader:
                    HandleStreamHeader(element);
                    break;

                case StreamNegotiationState.WaitingStreamFeatures:
                    return HandleStreamFeaturesAsync(element, cancellationToken);

                case StreamNegotiationState.NegotiatingFeature:
                    return HandleFeatureNegotiationMessageAsync(element, cancellationToken);
            }
            
            return Task.CompletedTask;
        }

        private void HandleStreamHeader(XmlElement streamHeader)
        {
            VerifyStreamHeader(streamHeader);
            State = StreamNegotiationState.WaitingStreamFeatures;
        }

        private Task HandleStreamFeaturesAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyFeaturesResponse(element);

            if (!element.HasChildren)
            {
                State = StreamNegotiationState.Done;
                return Task.CompletedTask;
            }

            //if (!HasNegotiatorsForFeatures(element.Children))
            //{
            //    return;
            //}

            State = StreamNegotiationState.NegotiatingFeature;
            var feature = SelectFeature(element.Children);
            //currentNegotiator = GetNegotiator(feature);
            return null;// currentNegotiator.Invoke(feature, cancellationToken);
        }

        private async Task HandleFeatureNegotiationMessageAsync(XmlElement message, CancellationToken cancellationToken)
        {
            //var state = await currentNegotiator.Invoke(message, cancellationToken);

            //if (state == FeatureNegotiationState.StreamRestartRequired)
            //{
            //    var header = CreateHeader(settings.Domain);
            //    await messageSender.SendAsync(header, cancellationToken);
            //    State = StreamNegotiationState.WaitingStreamHeader;
            //}

            //if (state == FeatureNegotiationState.Completed)
            //{
            //    State = StreamNegotiationState.WaitingStreamFeatures;
            //    currentNegotiator = null;
            //}
        }

        private XmlElement CreateHeader(string domain) => StreamHeader.Client().To(domain).NewId();

        private void VerifyStreamHeader(XmlElement header)
        {
            if (!IsStreamHeader(header))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:stream", header.Name));
            }
        }

        private void VerifyFeaturesResponse(XmlElement features)
        {
            if (!IsStreamFeatures(features))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:stream", features.Name));
            }
        }

        private bool IsStreamHeader(XmlElement element) => element.Name == "stream:stream";

        private bool IsStreamFeatures(XmlElement element) => element.Name == "stream:features";

        private IFeatureComponent GetNegotiator(XmlElement feature)
        {
            var component = FeatureComponents.FirstOrDefault(c => c.CanNegotiate(feature));

            if (component == null)
            {
                throw new InvalidOperationException(string.Format(Resources.NoNegotiatorForFeature, feature.Name));
            }

            return component;
        }

        private bool HasNegotiatorsForFeatures(IEnumerable<XmlElement> features)
        {
            return false;
            //return features.Select(f => f.Name)
            //    .Except(negotiatedFeatures)
            //    .Any(f => negotiators.ContainsKey(f));
        }

        private XmlElement SelectFeature(IEnumerable<XmlElement> features)
        {
            return default;
            //if (HasTlsFeature(features)
            //    && settings.UseTls
            //    && !negotiatedFeatures.Contains("starttls")
            //    && negotiators.ContainsKey("starttls"))
            //{
            //    return GetTlsFeature(features);
            //}

            //if (HasSaslFeature(features)
            //    && settings.UseSasl
            //    && !negotiatedFeatures.Contains("mechanisms")
            //    && negotiators.ContainsKey("mechanisms"))
            //{
            //    return GetSaslFeature(features);
            //}

            //return features.FirstOrDefault(
            //    f => negotiators.ContainsKey(f.Name)
            //    && !negotiatedFeatures.Contains(f.Name));
        }

        private bool HasTlsFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "starttls" && f.Xmlns() == XmppNamespaces.Tls);

        private XmlElement GetTlsFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "starttls");

        private bool HasSaslFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "mechanisms" && f.Xmlns() == XmppNamespaces.Sasl);

        private XmlElement GetSaslFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "mechanisms");
    }
}