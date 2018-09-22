namespace HyperMsg.Xmpp
{
    public static class Tls
    {
        public static XmlElement Start()
        {
            return new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);
        }

        public static XmlElement Proceed()
        {
            return new XmlElement("proceed").Xmlns(XmppNamespaces.Tls);
        }

        public static XmlElement Failure()
        {
            return new XmlElement("failure").Xmlns(XmppNamespaces.Tls);
        }
    }
}
