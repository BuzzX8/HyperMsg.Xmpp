using HyperMsg.Xmpp.Xml;
using System.Buffers;

namespace HyperMsg.Xmpp.Serialization
{
    public static class XmlDeserializer
    {
        internal static bool TryGetXmlElement(IBufferReader bufferReader, out XmlElement xmlElement)
        {
            xmlElement = default;
            return false;
        }

        public static (int BytesConsumed, XmlElement XmlElement) Deserialize(ReadOnlySequence<byte> buffer)
        {
            return (0, null);
        }
    }
}
