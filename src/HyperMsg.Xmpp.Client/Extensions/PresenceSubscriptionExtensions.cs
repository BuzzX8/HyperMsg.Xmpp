using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class PresenceSubscriptionExtensions
    {
        public static Task<string> SendSubscriptionRequestAsync(this IMessageSender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Subscribe().To(to));

        public static Task<string> SendSubscriptionApproveAsync(this IMessageSender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Subscribed().To(to));

        public static Task<string> SendSubscriptionDenyAsync(this IMessageSender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Unsubscribed().To(to));

        public static Task<string> SendSubscriptionCancellationAsync(this IMessageSender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Unsubscribed().To(to));

        public static Task<string> SendUnsubscribeRequestAsync(this IMessageSender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Unsubscribe().To(to));
    }
}
