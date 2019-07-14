using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IMessageService
    {
        Task SendMessageAsync(Jid to, Message message, CancellationToken cancellationToken);

        event AsyncAction<Message> MessageReceived;
    }
}