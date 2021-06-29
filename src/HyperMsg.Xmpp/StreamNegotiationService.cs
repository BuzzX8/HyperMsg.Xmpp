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
        private XmlElement currentFeature;
        private XmppConnectionSettings settings;

        public StreamNegotiationService(IMessagingContext context, IDataRepository dataRepository) : base(context) =>
            this.dataRepository = dataRepository;

        protected override IEnumerable<IDisposable> GetAutoDisposables()
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
                StreamNegotiationState.WaitingStreamFeatures => HandleStreamFeaturesAsync(element, cancellationToken),
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

        private async Task HandleStreamFeaturesAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyFeaturesResponse(element);

            if (!element.HasChildren || !HasNegotiatorsForFeatures(element.Children))
            {
                negotiationState = StreamNegotiationState.Done;
                return;
            }

            var feature = SelectFeature(element.Children);
            //currentNegotiator = GetNegotiator(feature);
            //var isRestartRequired = await await currentNegotiator.NegotiateAsync(MessagingContext, feature, cancellationToken);
            //currentFeature = feature;

            //await HandleFeatureNegotiationStateAsync(isRestartRequired, cancellationToken);            
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
                await this.SendToTransmitPipeAsync(header, cancellationToken);
                negotiationState = StreamNegotiationState.WaitingStreamHeader;
            }
            else
            {
                negotiationState = StreamNegotiationState.WaitingStreamFeatures;
                negotiatedFeatures.Add(currentFeature);
                currentFeature = null;
                //currentNegotiator = null;
            }
        }

        private async Task HandleFeatureNegotiationMessageAsync(XmlElement message, CancellationToken cancellationToken)
        {
            //var state = await await currentNegotiator.NegotiateAsync(MessagingContext, message, cancellationToken);

            //await HandleFeatureNegotiationStateAsync(state, cancellationToken);
        }

        private bool IsStreamFeatures(XmlElement element) => element.Name == "stream:features";

        private bool HasNegotiatorsForFeatures(IEnumerable<XmlElement> features)
        {
            return false;
            //return features
            //    .Except(negotiatedFeatures)
                //.Any(f => featureNegotiators.Any(c => c.CanNegotiate(f)));
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

        private bool HasNegotiatorForFeature(XmlElement feature) => false;// featureNegotiators.Any(c => c.CanNegotiate(feature));

        private MessagingService GetNegotiator(XmlElement feature)
        {
            //var component = featureNegotiators.FirstOrDefault(c => c.CanNegotiate(feature));

            //if (component == null)
            //{
            //    throw new InvalidOperationException($"NoNegotiatorForFeature {feature.Name}");
            //}

            return null;
        }
    }
}
