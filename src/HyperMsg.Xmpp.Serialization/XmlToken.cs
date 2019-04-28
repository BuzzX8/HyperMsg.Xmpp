namespace HyperMsg.Xmpp.Serialization
{
    public class XmlToken
    {
        internal XmlToken(string value, XmlTokenType type)
        {
            Type = type;
            Value = value;
        }

        public string Value { get; }

        public string TagName { get; internal set; }

        public XmlTokenType Type { get; }

        public override string ToString() => $"{Value}[{Type}]";
    }
}
