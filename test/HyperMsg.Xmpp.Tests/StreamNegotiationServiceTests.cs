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

        [Fact]
        public void Throws_Exception_If_Invalid_Features_Response_Received()
        {
            MessageSender.SendTransportMessage(TransportMessage.Opened);
            MessageSender.SendToReceivePipe(StreamHeader.Client());

            Assert.Throws<XmppException>(() => MessageSender.SendToReceivePipe(new XmlElement("stream:feature")));
        }

        [Fact]
        public void Can_Negotiate_Tls_Feature()
        {
            XmlElement sentElement = default;
            bool? setTlsSend = default;
            var featuresResponse = new XmlElement("stream:features", new XmlElement("starttls").Xmlns(XmppNamespaces.Tls));

            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(request => sentElement = request);
            HandlersRegistry.RegisterTransportMessageHandler(TransportMessage.SetTls, () => setTlsSend = true);

            MessageSender.SendTransportMessage(TransportMessage.Opened);
            MessageSender.SendToReceivePipe(StreamHeader.Client());
            MessageSender.SendToReceivePipe(featuresResponse);

            Assert.Equal("starttls", sentElement.Name);

            MessageSender.SendToReceivePipe(new XmlElement("proceed").Xmlns(XmppNamespaces.Tls));
            Assert.True(setTlsSend);
        }
    }
}
