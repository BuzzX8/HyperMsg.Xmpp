using System;

namespace HyperMsg.Xmpp.Serialization
{
    public class XmlToken
    {
        public XmlToken(XmlTokenType type, string name)
        {
            Type = type;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public XmlToken(XmlTokenType type, string name, string value) : this(type, name)
        {
            Value = value ?? throw new ArgumentNullException(value);
        }

        public XmlTokenType Type { get; }

        public string Name { get; }

        public string Value { get; }

        public override string ToString() => $"{Value}[{Type}]";
    }
}
