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
            throw new NotImplementedException();
        }

        public Task CancelSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RejectSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RequestSubscriptionAsync(Jid subscriptionJid, CancellationToken cancellationToken)
        {
            var stanza = CreateSubscriptionRequest(subscriptionJid);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        private XmlElement CreateSubscriptionRequest(Jid subscriptionJid)
        {
            return new XmlElement("presence")
                .To(subscriptionJid)
                .Type("subscribe");
        }

        public Task UnsubscribeAsync(Jid subscriptionJid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public event Action<Jid> SubscriptionRequested;

        public event Action<Jid> SubscriptionCanceled;

        public event Action<Jid> SubscriptionRejected;

        public event Action<Jid> Unsubscribed;
    }
}
