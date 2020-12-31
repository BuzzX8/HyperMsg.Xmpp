using HyperMsg.Extensions;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    internal class StanzaReceiveComponent 
    {
        private readonly IMessagingContext messagingContext;

        internal StanzaReceiveComponent(IMessagingContext messagingContext)
        {
            this.messagingContext = messagingContext;
        }

        internal Task HandleAsync(XmlElement xmlElement, CancellationToken cancellationToken)
        {
            if (xmlElement.IsIqStanza())
            {
                return HandleIqStanza(xmlElement, cancellationToken);
            }

            if (xmlElement.IsPresenceStanza())
            {
                return HandlePresenceStanza(xmlElement, cancellationToken);
            }

            if (xmlElement.IsMessageStanza())
            {
                return HandleMessageStanzaAsync(xmlElement, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private Task HandleIqStanza(XmlElement iqStanza, CancellationToken cancellationToken)
        {
            if (IsRosterResultStanza(iqStanza))
            {
                var queryElement = iqStanza.Child("query");
                var items = ToRosterItems(queryElement.Children);
                return messagingContext.Sender.ReceivedAsync(items, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private bool IsRosterResultStanza(XmlElement stanza) => stanza.IsType(IqStanza.Type.Result) && stanza.HasChild("query");

        private IReadOnlyList<RosterItem> ToRosterItems(IEnumerable<XmlElement> items) => items.Select(i => new RosterItem(i["jid"], i["name"])).ToArray();

        private Task HandlePresenceStanza(XmlElement presenceStanza, CancellationToken cancellationToken)
        {
            if (!IsPresenceStatusStanza(presenceStanza))
            {                
                var status = ToPresenceStatus(presenceStanza);
                return messagingContext.Sender.ReceivedAsync(status, cancellationToken);
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

        private Task HandleMessageStanzaAsync(XmlElement messageStanza, CancellationToken cancellationToken)
        {
            Enum.TryParse<MessageType>(messageStanza.Type(), true, out var type);

            var message = new Message
            {
                Type = type,
                Subject = messageStanza.Child("subject")?.Value,
                Body = messageStanza.Child("body")?.Value
            };

            return messagingContext.Sender.ReceivedAsync(message, cancellationToken);
        }
    }
}
