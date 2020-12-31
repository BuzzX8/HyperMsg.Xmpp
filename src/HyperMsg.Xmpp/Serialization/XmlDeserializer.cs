using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
    public static class XmlDeserializer
    {
        public static (int BytesConsumed, XmlElement XmlElement) Deserialize(ReadOnlySequence<byte> buffer)
        {
            return (0, null);
        }
    }
}
