using HyperMsg.Xmpp.Serialization;
using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    internal class BufferSerializationService : MessageService
    {
        public BufferSerializationService(IMessagingContext messagingContext) : base(messagingContext)
        { }

        protected override IEnumerable<IDisposable> GetChildDisposables()
        {
            yield return this.RegisterSerializationHandler<XmlElement>(XmlSerializer.Serialize);
            yield return this.RegisterReceiveBufferReaderHandler(HandleReceiveBufferReader);
        }

        private async Task HandleReceiveBufferReader(IBufferReader bufferReader, CancellationToken cancellationToken)
        {
            if (!XmlDeserializer.TryGetXmlElement(bufferReader, out var xmlElement))
            {
                return;
            }

            await this.SendToReceivePipeAsync(xmlElement, cancellationToken);
        }
    }
}
