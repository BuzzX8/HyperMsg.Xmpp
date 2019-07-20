using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class MessageService : IMessageService
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly Jid jid;

        public MessageService(IMessageSender<XmlElement> messageSender, Jid jid)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.jid = jid ?? throw new ArgumentNullException(nameof(jid));
        }

        public Task SendMessageAsync(Jid to, Message message, CancellationToken cancellationToken)
        {
            var messageStanza = CreateMessageStanza(to, message);
            return messageSender.SendAsync(messageStanza, cancellationToken);
        }

        private XmlElement CreateMessageStanza(Jid to, Message message)
        {
            var type = message.Type.ToString().ToLower();
            return MessageStanza.New(type, message.Subject, message.Body)
                .From(jid)
                .To(to);
        }

        public void Handle(XmlElement messageStanza)
        {
            if (!messageStanza.IsMessage())
            {
                return;
            }

            var message = ToMessage(messageStanza);
            MessageReceived?.Invoke(message);
        }

        private Message ToMessage(XmlElement messageStanza)
        {
            Enum.TryParse<MessageType>(messageStanza.Type(), true, out var type);

            return new Message
            {
                Type = type,
                Subject = messageStanza.Child("subject")?.Value,
                Body = messageStanza.Child("body")?.Value
            };
        }

        public event Action<Message> MessageReceived;
    }
}
