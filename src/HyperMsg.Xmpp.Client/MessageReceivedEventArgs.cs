using System;

namespace HyperMsg.Xmpp.Client
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string id, Jid senderJid, Message message)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            SenderJid = senderJid ?? throw new ArgumentNullException(nameof(senderJid));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Id { get; }

        public Jid SenderJid { get; }

        public Message Message { get; }
    }
}
