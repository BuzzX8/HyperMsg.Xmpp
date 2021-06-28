using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    public abstract class FeatureNegotiationService : MessagingService
    {
        protected FeatureNegotiationService(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        protected abstract bool CanNegotiate(XmlElement feature);

        protected abstract Task SendNegotiationRequestAsync(XmlElement feature, CancellationToken cancellationToken);

        protected abstract Task HandleResponseAsync(XmlElement response, CancellationToken cancellationToken);

        protected void SetNogotiationCompleted(bool isStreamRestartRequired)
        {
            
        }

        protected Task SetNogotiationCompletedAsync(bool isStreamRestartRequired, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
