using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    public abstract class FeatureNegotiationService : MessagingService
    {
        protected FeatureNegotiationService(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        protected override IEnumerable<IDisposable> GetAutoDisposables()
        {
            yield return RegisterHandler<FeatureNegotiatorRequest>(HandleNegotiatorRequest);
        }

        internal bool IsNegotiationCompleted { get; private set; }

        internal bool IsStreamRestartRequired { get; private set; }

        protected abstract bool CanNegotiate(XmlElement feature);

        protected abstract Task SendNegotiationRequestAsync(XmlElement feature, CancellationToken cancellationToken);

        protected abstract Task HandleResponseAsync(XmlElement response, CancellationToken cancellationToken);

        protected void SetNegotiationCompleted(bool isStreamRestartRequired)
        {
            IsNegotiationCompleted = true;
            IsStreamRestartRequired = isStreamRestartRequired;
        }

        internal Task SendNegotiationRequestInternal(XmlElement feature, CancellationToken cancellationToken)
        {
            IsNegotiationCompleted = IsStreamRestartRequired = false;
            return SendNegotiationRequestAsync(feature, cancellationToken);
        }

        internal Task HandleResponseInternal(XmlElement response, CancellationToken cancellationToken) =>
            HandleResponseAsync(response, cancellationToken);

        private void HandleNegotiatorRequest(FeatureNegotiatorRequest request)
        {
            if (!CanNegotiate(request.Feature))
            {
                return;
            }

            request.FeatureNegotiator = this;
        }
    }

    internal class FeatureNegotiatorRequest
    {
        public XmlElement Feature { get; set; }

        public FeatureNegotiationService FeatureNegotiator { get; set; }
    }
}
