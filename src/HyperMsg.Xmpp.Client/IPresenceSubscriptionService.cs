using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IPresenceSubscriptionService
    {
        Task RequestSubscriptionAsync(Jid subscriptionJid, CancellationToken cancellationToken);

        Task UnsubscribeAsync(Jid subscriptionJid, CancellationToken cancellationToken);

        Task ApproveSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken);

        Task RejectSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken);

        Task CancelSubscriptionAsync(Jid subscriberJid, CancellationToken cancellationToken);

        event Action<Jid> SubscriptionRequested;

        event Action<Jid> SubscriptionCanceled;

        event Action<Jid> SubscriptionRejected;

        event Action<Jid> Unsubscribed;
    }
}
