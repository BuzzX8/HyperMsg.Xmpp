using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Extensions
{
    public static class MessagingContextExtensions
    {
        public static Task<MessagingTask<bool>> OpenStreamAsync(this IMessagingContext messagingContext, XmppConnectionSettings connectionSettings, CancellationToken cancellationToken = default)
        {
            return new ConnectTask(messagingContext, cancellationToken).StartAsync(connectionSettings);
        }
    }
}
