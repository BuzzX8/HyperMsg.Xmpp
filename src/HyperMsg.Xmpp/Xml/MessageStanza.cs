namespace HyperMsg.Xmpp.Xml
{
    public static class MessageStanza
    {
        public static class Type
        {
            public static readonly string Chat = "chat";
            public static readonly string Error = "error";
            public static readonly string GroupChat = "groupchat";
            public static readonly string Headline = "headline";
            public static readonly string Normal = "normal";
        }

        public static XmlElement New(string type, string subject = null, string body = null)
        {
            var stanza = new XmlElement("message").Type(type);

            if (!string.IsNullOrEmpty(subject))
            {
                AddSubject(stanza, subject);
            }

            if (!string.IsNullOrEmpty(body))
            {
                AddBody(stanza, body);
            }

            return stanza;
        }

        private static XmlElement AddSubject(XmlElement stanza, string subject) => stanza.Children(new XmlElement("subject").Value(subject));

        private static XmlElement AddBody(XmlElement stanza, string body) => stanza.Children(new XmlElement("body").Value(body));
    }
}
