using HyperMsg.Transport;
using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    internal class StreamNegotiationService : MessagingService
    {
        private readonly IDataRepository dataRepository;
                
        private StreamNegotiationState negotiationState;        
        private List<XmlElement> negotiatedFeatures;
        private FeatureNegotiator currentNegotiator;
        private XmlElement currentFeature;
        private XmppConnectionSettings settings;

        public StreamNegotiationService(IMessagingContext context, IDataRepository dataRepository) : base(context) =>
            this.dataRepository = dataRepository;

        protected override IEnumerable<IDisposable> GetChildDisposables()
        {
            yield return this.RegisterTransportMessageHandler(TransportMessage.Opened, StartNegotiationAsync);
            yield return this.RegisterReceivePipeHandler<XmlElement>(HandleXmlElementAsync);
        }

        private async Task StartNegotiationAsync(CancellationToken cancellationToken)
        {
            if (!dataRepository.TryGet(out settings))
            {
                return;
            }
            
            negotiatedFeatures = new List<XmlElement>();
            var header = CreateHeader(settings.Domain);

            await this.SendToTransmitPipeAsync(header, cancellationToken);
            negotiationState = StreamNegotiationState.WaitingStreamHeader;
        }

        private XmlElement CreateHeader(string domain) => StreamHeader.Client().To(domain).NewId();

        private Task HandleXmlElementAsync(XmlElement element, CancellationToken cancellationToken)
        {
            return negotiationState switch
            {
                StreamNegotiationState.WaitingStreamHeader => HandleStreamHeaderResponseAsync(element),
                StreamNegotiationState.WaitingStreamFeatures => HandleStreamFeaturesResponseAsync(element, cancellationToken),
                StreamNegotiationState.NegotiatingFeature => HandleFeatureNegotiationMessageAsync(element, cancellationToken),
                _ => Task.CompletedTask
            };
        }

        private Task HandleStreamHeaderResponseAsync(XmlElement streamHeader)
        {
            VerifyStreamHeader(streamHeader);
            negotiationState = StreamNegotiationState.WaitingStreamFeatures;
            return Task.CompletedTask;
        }

        private void VerifyStreamHeader(XmlElement header)
        {
            if (!IsStreamHeader(header))
            {
                throw new XmppException($"InvalidXmlElementReceived. Expected stream:stream but received {header.Name}");
            }
        }

        private bool IsStreamHeader(XmlElement element) => element.Name == "stream:stream";

        private async Task HandleStreamFeaturesResponseAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyFeaturesResponse(element);

            if (!element.HasChildren || !HasNegotiatorsForFeatures(element.Children))
            {
                negotiationState = StreamNegotiationState.Done;
                return;
            }

            (currentFeature, currentNegotiator) = GetFeatureWithNegotiator(element.Children);
            negotiationState = StreamNegotiationState.NegotiatingFeature;
            await currentNegotiator.SendNegotiationRequestInternal(currentFeature, cancellationToken);
        }

        private void VerifyFeaturesResponse(XmlElement features)
        {
            if (!IsStreamFeatures(features))
            {
                throw new XmppException($"InvalidXmlElementReceived. Expected stream:features, but received {features.Name}");
            }
        }

        private bool IsStreamFeatures(XmlElement element) => element.Name == "stream:features";

        private async Task HandleFeatureNegotiationMessageAsync(XmlElement message, CancellationToken cancellationToken)
        {
            await currentNegotiator.HandleResponseInternal(message, cancellationToken);
            if (currentNegotiator.IsNegotiationCompleted)
            {
                await HandleFeatureNegotiationStateAsync(currentNegotiator.IsStreamRestartRequired, cancellationToken);
                currentFeature = null;
                currentNegotiator = null;
            }
        }

        private async Task HandleFeatureNegotiationStateAsync(bool isRestartRequired, CancellationToken cancellationToken)
        {
            if (isRestartRequired)
            {
                var header = CreateHeader(settings.Domain);
                await this.SendToTransmitPipeAsync(header, cancellationToken);
                negotiationState = StreamNegotiationState.WaitingStreamHeader;
            }
            else
            {
                negotiationState = StreamNegotiationState.WaitingStreamFeatures;
                negotiatedFeatures.Add(currentFeature);
            }
        }        

        private bool HasNegotiatorsForFeatures(IEnumerable<XmlElement> features)
        {
            foreach(var feature in features)
            {
                if (TryGetNegotiatorForFeature(feature, out _))
                {
                    return true;
                }
            }

            return false;
        }

        private (XmlElement Feature, FeatureNegotiator Negotiator) GetFeatureWithNegotiator(IEnumerable<XmlElement> features)
        {
            var featureNegotiator = default(FeatureNegotiator);

            if (HasTlsFeature(features)
                && settings.UseTls
                && !negotiatedFeatures.Any(f => f.Name == "starttls"))
            {
                var tlsFeature = GetTlsFeature(features);

                if (TryGetNegotiatorForFeature(tlsFeature, out featureNegotiator))
                {
                    return (tlsFeature, featureNegotiator);
                }
            }

            if (HasSaslFeature(features)
                && settings.UseSasl
                && !negotiatedFeatures.Any(f => f.Name == "mechanisms"))
            {
                var saslFeature = GetSaslFeature(features);

                if (TryGetNegotiatorForFeature(saslFeature, out featureNegotiator))
                {
                    return (saslFeature, featureNegotiator);
                }
            }

            foreach (var feature in features)
            {
                if (!negotiatedFeatures.Contains(feature) && TryGetNegotiatorForFeature(feature, out featureNegotiator))
                {
                    return (feature, featureNegotiator);
                }
            }

            return (null, null);
        }

        private bool HasTlsFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "starttls" && f.Xmlns() == XmppNamespaces.Tls);

        private XmlElement GetTlsFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "starttls");

        private bool HasSaslFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "mechanisms" && f.Xmlns() == XmppNamespaces.Sasl);

        private XmlElement GetSaslFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "mechanisms");

        private bool TryGetNegotiatorForFeature(XmlElement feature, out FeatureNegotiator featureNegotiator)
        {
            var request = new FeatureNegotiatorRequest { Feature = feature };
            Send(request);
            featureNegotiator = request.FeatureNegotiator;
            return featureNegotiator is not null;
        }
    }
}
