using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class PresenceSubscriptionExtensions
    {
        public static string SendSubscriptionRequest(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewId(Presence.Subscribe().To(to));

        public static Task<string> SendSubscriptionRequestAsync(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Subscribe().To(to));

        public static string SendSubscriptionApprove(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewId(Presence.Subscribed().To(to));

        public static Task<string> SendSubscriptionApproveAsync(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Subscribed().To(to));

        public static string SendSubscriptionDeny(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewId(Presence.Unsubscribed().To(to));

        public static Task<string> SendSubscriptionDenyAsync(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Unsubscribed().To(to));

        public static string SendSubscriptionCancellation(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewId(Presence.Unsubscribed().To(to));

        public static Task<string> SendSubscriptionCancellationAsync(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Unsubscribed().To(to));

        public static string SendUnsubscribeRequest(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewId(Presence.Unsubscribe().To(to));

        public static Task<string> SendUnsubscribeRequestAsync(this ISender<XmlElement> sender, Jid to) => sender.SendWithNewIdAsync(Presence.Unsubscribe().To(to));
    }
}
