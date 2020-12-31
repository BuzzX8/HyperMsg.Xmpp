using HyperMsg.Xmpp.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Extensions
{
    internal class ConnectTask : MessagingTask<bool>
    {
        //private IFeatureComponent currentNegotiator;
        private StreamNegotiationState negotiationState;
        private List<XmlElement> negotiatedFeatures;
        private XmlElement currentFeature;

        public ConnectTask(IMessagingContext context, CancellationToken cancellationToken = default) : base(context, cancellationToken)
        {
            AddReceiver<XmlElement>(HandleAsync);
        }

        internal async Task<MessagingTask<bool>> StartAsync(XmppConnectionSettings connectionSettings)
        {
            var header = CreateHeader(connectionSettings.Domain);
            await TransmitAsync(header, CancellationToken);
            negotiationState = StreamNegotiationState.WaitingStreamHeader;

            return this;
        }

        private XmlElement CreateHeader(string domain) => StreamHeader.Client().To(domain).NewId();

        private Task HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            switch (negotiationState)
            {
                case StreamNegotiationState.WaitingStreamHeader:
                    return Task.FromResult(HandleStreamHeader(element));

                case StreamNegotiationState.WaitingStreamFeatures:
                    return HandleStreamFeaturesAsync(element, cancellationToken);

                    //case StreamNegotiationState.NegotiatingFeature:
                    //    return HandleFeatureNegotiationMessageAsync(element, cancellationToken);
            }

            return Task.FromResult(negotiationState);
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

        private async Task<StreamNegotiationState> HandleStreamFeaturesAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyFeaturesResponse(element);

            //if (!element.HasChildren || !HasNegotiatorsForFeatures(element.Children))
            //{
            //    return negotiationState = StreamNegotiationState.Done;
            //}

            //var feature = SelectFeature(element.Children);
            //currentNegotiator = GetNegotiator(feature);
            //var state = await currentNegotiator.StartNegotiationAsync(feature, cancellationToken);
            //currentFeature = feature;
            //return await HandleFeatureNegotiationStateAsync(state, cancellationToken);
            return StreamNegotiationState.NegotiatingFeature;
        }

        private void VerifyFeaturesResponse(XmlElement features)
        {
            if (!IsStreamFeatures(features))
            {
                throw new XmppException($"InvalidXmlElementReceived. Expected stream:features, but received {features.Name}");
            }
        }

        private bool IsStreamFeatures(XmlElement element) => element.Name == "stream:features";
    }
}
