namespace HyperMsg.Xmpp
{
    public static class Presence
    {
        public static class ShowStatus
        {
            public static readonly string Away = "away";
            public static readonly string Chat = "chat";
            public static readonly string DoNotDisturb = "dnd";
            public static readonly string ExtendedAway = "xa";
        }

        public static class Type
        {
            public static readonly string Error = "error";
            public static readonly string Probe = "probe";
            public static readonly string Subscribe = "subscribe";
            public static readonly string Subscribed = "subscribed";
            public static readonly string Unavailable = "unavailable";
            public static readonly string Unsubscribe = "unsubscribe";
            public static readonly string Unsubscribed = "unsubscribed";
        }

        public static XmlElement New()
        {
            return new XmlElement("presence");
        }

        public static XmlElement Available()
        {
            return New();
        }

        public static XmlElement Unavailable()
        {
            return New().Type(Type.Unavailable);
        }

        public static XmlElement Subscribe()
        {
            return New().Type(Type.Subscribe);
        }

        public static XmlElement Subscribed()
        {
            return New().Type(Type.Subscribed);
        }

        public static XmlElement Unsubscribe()
        {
            return New().Type(Type.Unsubscribe);
        }

        public static XmlElement Unsubscribed()
        {
            return New().Type(Type.Unsubscribed);
        }

        public static XmlElement Error()
        {
            return New().Type(Type.Error);
        }

        public static XmlElement Probe()
        {
            return New().Type(Type.Probe);
        }

        public static XmlElement Show(this XmlElement presence, string showStatus)
        {
            return presence.Children(new XmlElement("show").Value(showStatus));
        }

        public static XmlElement ShowAway(this XmlElement presence)
        {
            return presence.Show(ShowStatus.Away);
        }

        public static XmlElement ShowChat(this XmlElement presence)
        {
            return presence.Show(ShowStatus.Chat);
        }

        public static XmlElement ShowDoNotDisturb(this XmlElement presence)
        {
            return presence.Show(ShowStatus.DoNotDisturb);
        }

        public static XmlElement ShowExtendedAway(this XmlElement presence)
        {
            return presence.Show(ShowStatus.ExtendedAway);
        }

        public static XmlElement Status(this XmlElement presence, string status)
        {
            return presence.Children(new XmlElement("status").Value(status));
        }
    }
}