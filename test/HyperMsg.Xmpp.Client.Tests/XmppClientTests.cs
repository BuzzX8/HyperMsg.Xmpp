using FakeItEasy;
using HyperMsg.Xmpp.Client.StreamNegotiation;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class XmppClientTests
    {
        private readonly XmppClient client;
        private readonly IStreamNegotiator streamNegotiator;
        private readonly ITransceiver<XmlElement, XmlElement> transceiver;
        private readonly XmppConnectionSettings settings;
        private readonly IHandler<TransportCommands> transportHandler;
        private readonly IHandler<ReceiveMode> receiveModeHandler;

        private readonly CancellationToken cancellationToken;

        public XmppClientTests()
        {
            streamNegotiator = A.Fake<IStreamNegotiator>();
            transceiver = A.Fake<ITransceiver<XmlElement, XmlElement>>();
            settings = new XmppConnectionSettings("user@domain");
            transportHandler = A.Fake<IHandler<TransportCommands>>();
            receiveModeHandler = A.Fake<IHandler<ReceiveMode>>();
            client = new XmppClient(streamNegotiator, transceiver, settings, transportHandler, receiveModeHandler);

            cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task ConnectAsync_Submits_OpenConnection_Command()
        {
            await client.ConnectAsync(cancellationToken);

            A.CallTo(() => transportHandler.HandleAsync(TransportCommands.OpenConnection, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task ConnectAsync_Negotiates_Stream_With_Stream_Negotiator()
        {
            await client.ConnectAsync(cancellationToken);

            A.CallTo(() => streamNegotiator.NegotiateAsync(transceiver, settings, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task ConnectAsync_Switches_To_Reactive_Mode()
        {
            await client.ConnectAsync(cancellationToken);

            A.CallTo(() => receiveModeHandler.HandleAsync(ReceiveMode.Reactive, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task DisconnectAsync_Sends_EndOfStream_Element()
        {
            await client.DisconnectAsync(cancellationToken);

            A.CallTo(() => transceiver.SendAsync(new XmlElement("/stream:stream"), cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task DisconnectAsync_Submits_CloseConnection_Command()
        {
            await client.DisconnectAsync(cancellationToken);

            A.CallTo(() => transportHandler.HandleAsync(TransportCommands.CloseConnection, cancellationToken)).MustHaveHappened();
        }
    }
}
