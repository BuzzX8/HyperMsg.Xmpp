using HyperMsg.Transport;
using HyperMsg.Xmpp.Xml;
using Xunit;

namespace HyperMsg.Xmpp
{
    public class StreamNegotiationServiceTests : ServiceHostFixture
    {
        private readonly IDataRepository dataRepository;
        private readonly XmppConnectionSettings settings;        

        public StreamNegotiationServiceTests() : base(services => services.AddXmppServices())
        {
            dataRepository = GetRequiredService<IDataRepository>();
            settings = new XmppConnectionSettings("test_jid@domain.test");
            dataRepository.AddOrReplace(settings);
        }

        [Fact]
        public void Sends_Stream_Header_To_Transmit_Pipe()
        {
            var sentHeader = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(request => sentHeader = request);

            MessageSender.SendTransportMessage(TransportMessage.Opened);

            Assert.NotNull(sentHeader);
            Assert.Equal("stream:stream", sentHeader.Name);
        }

        [Fact]
        public void Throws_Exception_If_Invalid_Header_Received()
        {
            MessageSender.SendTransportMessage(TransportMessage.Opened);

            Assert.Throws<XmppException>(() => MessageSender.SendToReceivePipe(new XmlElement("stream:strea")));
        }
    }
}
