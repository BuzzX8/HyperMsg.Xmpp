﻿using System.Buffers;

namespace HyperMsg.Xmpp.Serialization
{
    public class XmppSerializer : ISerializer<XmlElement>
    {
        public DeserializationResult<XmlElement> Deserialize(ReadOnlySequence<byte> buffer) => buffer.ReadXmlElement();

        public void Serialize(IBufferWriter<byte> writer, XmlElement message) => writer.WriteXmlElement(message);
    }
}