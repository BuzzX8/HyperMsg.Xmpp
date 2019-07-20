using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class PresenceSubscriptionService : IPresenceSubscriptionService
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly Jid jid;

        public PresenceSubscriptionService(IMessageSender<XmlElement> messageSender, Jid jid)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.jid = jid ?? throw new ArgumentNullException(nameof(jid));
        }

        public Task ApproveSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriberJid, "subscribed");
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public Task CancelSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriberJid, "unsubscribed");
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public Task RequestSubscriptionAsync(Jid subscriptionJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriptionJid, "subscribe");
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        public Task UnsubscribeAsync(Jid subscriptionJid, CancellationToken cancellationToken)
        {
            var stanza = CreatePresenceStanza(subscriptionJid, "unsubscribe");
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        private XmlElement CreatePresenceStanza(Jid to, string type)
        {
            return new XmlElement("presence")
                .NewId()
                .To(to)
                .Type(type);
        }

        public event Action<Jid> SubscriptionRequested;

        public event Action<Jid> SubscriptionCanceled;

        public event Action<Jid> SubscriptionRejected;

        public event Action<Jid> Unsubscribed;
    }
}
