namespace HyperMsg.Xmpp.Xml
{
    public static class IqStanza
    {
        public static class Type
        {
            public static readonly string Get = "get";
            public static readonly string Set = "set";
            public static readonly string Result = "result";
            public static readonly string Error = "error";
        }

        public static XmlElement New() => new XmlElement("iq");

        public static XmlElement Get() => New().Type(Type.Get);

        public static XmlElement Set() => New().Type(Type.Set);

        public static XmlElement Result() => New().Type(Type.Result);

        public static XmlElement Error() => New().Type(Type.Error);
    }
}
