using System.Buffers;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
    public struct XmlToken
    {
        public static readonly XmlToken Empty = new XmlToken(new ReadOnlySequence<byte>(), XmlTokenType.None);

        public XmlToken(ReadOnlySequence<byte> bufferSegments, XmlTokenType type)
        {
            BufferSegments = bufferSegments;
            Type = type;
        }

        public ReadOnlySequence<byte> BufferSegments { get; }

        public XmlTokenType Type { get; }

        public override int GetHashCode()
        {
            return Type.GetHashCode()
                ^ BufferSegments.GetHashCode();
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
                && BufferSegments.Equals(token.BufferSegments);
        }

        public override string ToString()
        {
            var bytes = BufferSegments.ToArray();
            var value = Encoding.UTF8.GetString(bytes);

            return $"[{Type}]{value}";
        }
    }
}
