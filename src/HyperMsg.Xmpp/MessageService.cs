using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    internal class MessageService : MessagingService
    {
        public MessageService(IMessagingContext messagingContext) : base(messagingContext)
        { }

        protected override IEnumerable<IDisposable> GetChildDisposables()
        {
            yield return this.RegisterReceivePipeHandler<XmlElement>(HandleStanzaResponse);
        }

        private Task HandleStanzaResponse(XmlElement xmlElement, CancellationToken cancellationToken)
        {
            if (!xmlElement.IsIqStanza())
            {
                return Task.CompletedTask;
            }

            return HandleMessageStanzaAsync(xmlElement, cancellationToken);
        }

        private Task HandleMessageStanzaAsync(XmlElement messageStanza, CancellationToken cancellationToken)
        {
            Enum.TryParse<MessageType>(messageStanza.Type(), true, out var type);

            var message = new Message
            {
                Type = type,
                Subject = messageStanza.Child("subject")?.Value,
                Body = messageStanza.Child("body")?.Value
            };

            return this.SendToReceivePipeAsync(message, cancellationToken);
        }
    }
}
