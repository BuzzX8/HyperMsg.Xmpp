using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IMessageService
    {
        Task<string> SendMessageAsync(Jid recipientJid, Message message, CancellationToken cancellationToken);

        event Action<MessageReceivedEventArgs> MessageReceived;
    }
}