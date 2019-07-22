using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class MessageService : IMessageService
    {
        private readonly IMessageSender<XmlElement> messageSender;

        public MessageService(IMessageSender<XmlElement> messageSender)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public async Task<string> SendMessageAsync(Jid recipientJid, Message message, CancellationToken cancellationToken)
        {
            var messageStanza = CreateMessageStanza(recipientJid, message);
            await messageSender.SendAsync(messageStanza, cancellationToken);
            return messageStanza.Id();
        }

        private XmlElement CreateMessageStanza(Jid recipientJid, Message message)
        {
            var type = message.Type.ToString().ToLower();

            return MessageStanza.New(type, message.Subject, message.Body)
                .NewId()
                .To(recipientJid);
        }

        public void Handle(XmlElement messageStanza)
        {
            if (!messageStanza.IsMessageStanza())
            {
                return;
            }

            var id = messageStanza.Id();
            var senderJid = Jid.Parse(messageStanza["from"]);
            var message = ToMessage(messageStanza);

            OnMessageReceived(new MessageReceivedEventArgs(id, senderJid, message));
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

        private void OnMessageReceived(MessageReceivedEventArgs eventArgs)
        {
            MessageReceived?.Invoke(eventArgs);
        }

        public event Action<MessageReceivedEventArgs> MessageReceived;
    }
}
