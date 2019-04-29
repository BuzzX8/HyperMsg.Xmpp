using System;

namespace HyperMsg.Xmpp.Serialization
{
    public struct XmlToken
    {
        public XmlToken(XmlTokenType type, string name)
        {
            Type = type;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = string.Empty;
        }

        public XmlToken(XmlTokenType type, string name, string value) : this(type, name)
        {
            Value = value ?? throw new ArgumentNullException(value);
        }

        public XmlTokenType Type { get; }

        public string Name { get; }

        public string Value { get; }

        public override int GetHashCode()
        {
            return Type.GetHashCode()
                ^ Name.GetHashCode()
                ^ Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(XmlToken))
            {
                return false;
            }

            return Equals((XmlToken)obj);
        }

        public bool Equals(XmlToken token)
        {
            return Type.Equals(token.Type)
                && Name.Equals(token.Name)
                && Value.Equals(token.Value);
        }

        public override string ToString() => $"{nameof(Type)}: {Type};{nameof(Name)}: {Name};{nameof(Value)}: {Value}";
    }
}
