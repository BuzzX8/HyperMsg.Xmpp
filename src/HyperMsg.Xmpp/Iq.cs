namespace HyperMsg.Xmpp
{
    public static class Iq
    {
        public static class Type
        {
            public static readonly string Get = "get";
            public static readonly string Set = "set";
            public static readonly string Result = "result";
            public static readonly string Error = "error";
        }

        public static XmlElement New()
        {
            return new XmlElement("iq");
        }

        public static XmlElement Get()
        {
            return New().Type(Type.Get);
        }

        public static XmlElement Set()
        {
            return New().Type(Type.Set);
        }

        public static XmlElement Result()
        {
            return New().Type(Type.Result);
        }

        public static XmlElement Error()
        {
            return New().Type(Type.Error);
        }
    }
}
