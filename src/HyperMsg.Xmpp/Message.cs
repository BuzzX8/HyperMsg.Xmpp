namespace HyperMsg.Xmpp
{
    public static class Message
    {
        public static class Type
        {
            public static readonly string Chat = "chat";
            public static readonly string Error = "error";
            public static readonly string GroupChat = "groupchat";
            public static readonly string Headline = "headline";
            public static readonly string Normal = "normal";
        }

        public static XmlElement New()
        {
            return new XmlElement("message");
        }

        public static XmlElement Chat()
        {
            return New().Type(Type.Chat);
        }

        public static XmlElement Error()
        {
            return New().Type(Type.Error);
        }

        public static XmlElement GroupChat()
        {
            return New().Type(Type.GroupChat);
        }

        public static XmlElement Headline()
        {
            return New().Type(Type.Headline);
        }

        public static XmlElement Normal()
        {
            return New().Type(Type.Normal);
        }

        public static XmlElement Subject(this XmlElement stanza, string subject)
        {
            return stanza.Children(new XmlElement("subject").Value(subject));
        }

        public static XmlElement Body(this XmlElement stanza, string body)
        {
            return stanza.Children(new XmlElement("body").Value(body));
        }
    }
}
