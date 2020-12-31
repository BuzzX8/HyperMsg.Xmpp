using HyperMsg.Xmpp.Client.Properties;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    public class ConnectionComponent
    {
        private readonly IMessageSender messageSender;
        private readonly XmppConnectionSettings settings;
                
        private IFeatureComponent currentNegotiator;
        private StreamNegotiationState negotiationState;
        private List<XmlElement> negotiatedFeatures;
        private XmlElement currentFeature;

        public ConnectionComponent(IMessageSender messageSender, XmppConnectionSettings settings)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            negotiatedFeatures = new List<XmlElement>();
        }

        public IList<IFeatureComponent> FeatureComponents { get; } = new List<IFeatureComponent>();

        public Task HandleTransportEventAsync(TransportEvent @event, CancellationToken cancellationToken)
        {
            if (@event == TransportEvent.Opened)
            {
                return OpenStreamAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public async Task OpenStreamAsync(CancellationToken cancellationToken)
        {
            var header = CreateHeader(settings.Domain);
            await messageSender.TransmitAsync(header, cancellationToken);
            negotiationState = StreamNegotiationState.WaitingStreamHeader;
            negotiatedFeatures.Clear();
        }

        public Task HandleAsync(Received<XmlElement> element, CancellationToken cancellationToken)
        {
            switch (negotiationState)
            {
                case StreamNegotiationState.WaitingStreamHeader:
                    return Task.FromResult(HandleStreamHeader(element));

                case StreamNegotiationState.WaitingStreamFeatures:
                    return HandleStreamFeaturesAsync(element, cancellationToken);

                case StreamNegotiationState.NegotiatingFeature:
                    return HandleFeatureNegotiationMessageAsync(element, cancellationToken);
            }
            
            return Task.FromResult(negotiationState);
        }

        private StreamNegotiationState HandleStreamHeader(XmlElement streamHeader)
        {
            VerifyStreamHeader(streamHeader);
            return negotiationState = StreamNegotiationState.WaitingStreamFeatures;
        }

        private async Task<StreamNegotiationState> HandleStreamFeaturesAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyFeaturesResponse(element);

            if (!element.HasChildren || !HasNegotiatorsForFeatures(element.Children))
            {
                return negotiationState = StreamNegotiationState.Done;
            }

            var feature = SelectFeature(element.Children);
            currentNegotiator = GetNegotiator(feature);
            var state = await currentNegotiator.StartNegotiationAsync(feature, cancellationToken);
            currentFeature = feature;
            return await HandleFeatureNegotiationStateAsync(state, cancellationToken);
        }

        private async Task<StreamNegotiationState> HandleFeatureNegotiationMessageAsync(XmlElement message, CancellationToken cancellationToken)
        {
            var state = await currentNegotiator.HandleAsync(message, cancellationToken);

            return await HandleFeatureNegotiationStateAsync(state, cancellationToken);
        }

        private async Task<StreamNegotiationState> HandleFeatureNegotiationStateAsync(FeatureNegotiationState state, CancellationToken cancellationToken)
        {
            switch (state)
            {
                case FeatureNegotiationState.Negotiating:
                    negotiationState = StreamNegotiationState.NegotiatingFeature;
                    break;

                case FeatureNegotiationState.Completed:
                    negotiationState = StreamNegotiationState.WaitingStreamFeatures;
                    negotiatedFeatures.Add(currentFeature);
                    currentFeature = null;
                    currentNegotiator = null;
                    break;

                case FeatureNegotiationState.StreamRestartRequire:
                    var header = CreateHeader(settings.Domain);
                    await messageSender.SendAsync(header, cancellationToken);
                    negotiationState = StreamNegotiationState.WaitingStreamHeader;
                    break;
            }

            return negotiationState;
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
            return features
                .Except(negotiatedFeatures)
                .Any(f => FeatureComponents.Any(c => c.CanNegotiate(f)));
        }

        private bool HasNegotiatorForFeature(XmlElement feature) => FeatureComponents.Any(c => c.CanNegotiate(feature));
        
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
    }
}