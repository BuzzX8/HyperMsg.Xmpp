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
            return new XmlElement("message")
                .To(to)
                .Type(type)
                .Subject(message.Subject)
                .Body(message.Body);
        }

        public async Task HandleAsync(XmlElement messageStanza, CancellationToken cancellationToken)
        {

        }

        public event AsyncAction<Message> MessageReceived;
    }
}
