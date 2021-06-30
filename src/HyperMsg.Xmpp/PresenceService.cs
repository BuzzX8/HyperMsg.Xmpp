using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    internal class PresenceService : MessagingService
    {
        public PresenceService(IMessagingContext messagingContext) : base(messagingContext)
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

            return HandlePresenceStanza(xmlElement, cancellationToken);
        }

        private Task HandlePresenceStanza(XmlElement presenceStanza, CancellationToken cancellationToken)
        {
            if (!IsPresenceStatusStanza(presenceStanza))
            {
                var status = ToPresenceStatus(presenceStanza);
                return this.SendToReceivePipeAsync(status, cancellationToken);
            }

            if (!IsPresenceSubscriptionStanza(presenceStanza))
            {
                var entityJid = Jid.Parse(presenceStanza["from"]);

                return presenceStanza.Type() switch
                {
                    "subscribe" => Task.CompletedTask,
                    "subscribed" => Task.CompletedTask,
                    "unsubscribed" => Task.CompletedTask,
                    _ => Task.CompletedTask
                };
            }

            return Task.CompletedTask;
        }

        private bool IsPresenceSubscriptionStanza(XmlElement stanza)
        {
            return stanza.IsPresenceStanza()
                && (stanza.IsType(PresenceStanza.Type.Subscribe)
                    || stanza.IsType(PresenceStanza.Type.Subscribed)
                    || stanza.IsType(PresenceStanza.Type.Unsubscribed));
        }

        private bool IsPresenceStatusStanza(XmlElement stanza)
        {
            return stanza.IsPresenceStanza()
                && (stanza.IsType(PresenceStanza.Type.Unavailable) || string.IsNullOrEmpty(stanza["type"]));
        }

        private PresenceStatus ToPresenceStatus(XmlElement presenceStanza)
        {
            Enum.TryParse<AvailabilitySubstate>(presenceStanza.Child("show").Value, true, out var substate);
            return new PresenceStatus
            {
                Jid = Jid.Parse(presenceStanza["from"]),
                AvailabilitySubstate = substate,
                StatusText = presenceStanza.Child("status").Value
            };
        }
    }
}
