using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    public class PresenceSubscriptionComponent : IPresenceSubscriptionService
    {
        private readonly IMessageSender messageSender;

        public PresenceSubscriptionComponent(IMessageSender messageSender)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public Task ApproveSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriberJid, PresenceStanza.Type.Subscribed);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public Task CancelSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriberJid, PresenceStanza.Type.Unsubscribed);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public Task RequestSubscriptionAsync(Jid subscriptionJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriptionJid, PresenceStanza.Type.Subscribe);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public Task UnsubscribeAsync(Jid subscriptionJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriptionJid, PresenceStanza.Type.Unsubscribe);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public void Handle(XmlElement presenceStanza)
        {
            if (!IsPresenceSubscriptionStanza(presenceStanza))
            {
                return;
            }

            var entityJid = Jid.Parse(presenceStanza["from"]);
            var type = presenceStanza.Type();
            
            switch(type)
            {
                case "subscribe":
                    SubscriptionRequested?.Invoke(entityJid);
                    break;

                case "subscribed":
                    SubscriptionApproved?.Invoke(entityJid);
                    break;

                case "unsubscribed":
                    SubscriptionCanceled?.Invoke(entityJid);
                    break;
            }
        }

        private bool IsPresenceSubscriptionStanza(XmlElement stanza)
        {
            return stanza.IsPresenceStanza()
                && (stanza.IsType(PresenceStanza.Type.Subscribe)
                    || stanza.IsType(PresenceStanza.Type.Subscribed)
                    || stanza.IsType(PresenceStanza.Type.Unsubscribed));
        }

        private XmlElement CreatePresenceStanza(Jid to, string type) => PresenceStanza.New(type).NewId().To(to);

        public event Action<Jid> SubscriptionApproved;

        public event Action<Jid> SubscriptionRequested;

        public event Action<Jid> SubscriptionCanceled;
    }
}
