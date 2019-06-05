namespace HyperMsg.Xmpp
{
    public static class Tls
    {
        public static readonly XmlElement Start = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);

        public static readonly XmlElement Proceed = new XmlElement("proceed").Xmlns(XmppNamespaces.Tls);

        public static readonly XmlElement Failure = new XmlElement("failure").Xmlns(XmppNamespaces.Tls);
    }
}
