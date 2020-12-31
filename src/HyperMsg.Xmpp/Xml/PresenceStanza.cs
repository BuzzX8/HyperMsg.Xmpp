using HyperMsg.Xmpp.Extensions;

namespace HyperMsg.Xmpp.Xml
{
    public static class PresenceStanza
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

        public static XmlElement New(string type, string showStatus = null, string statusText = null)
        {
            var stanza = new XmlElement("presence").Type(type);

            if (!string.IsNullOrEmpty(showStatus))
            {
                AddShowStatus(stanza, showStatus);
            }

            if (!string.IsNullOrEmpty(statusText))
            {
                AddStatusText(stanza, statusText);
            }

            return stanza;
        }

        private static void AddShowStatus(XmlElement presence, string showStatus) => presence.Children.Add(new XmlElement("show").Value(showStatus));

        private static void AddStatusText(XmlElement presence, string status) => presence.Children.Add(new XmlElement("status").Value(status));
    }
}