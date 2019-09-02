using FakeItEasy;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class TlsFeatureComponentTests
    {
        private readonly XmlElement startTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);

        private readonly IMessageSender<XmlElement> messageSender;
        private readonly AsyncAction<TransportCommand> transportCommandHandler;
        private readonly TlsFeatureComponent negotiator;

        private readonly CancellationTokenSource tokenSource;

        public TlsFeatureComponentTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            transportCommandHandler = A.Fake<AsyncAction<TransportCommand>>();
            negotiator = new TlsFeatureComponent(transportCommandHandler, messageSender);
            tokenSource = new CancellationTokenSource();
        }

        [Fact]
        public async Task StartNegotiationAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.StartNegotiationAsync(feature, tokenSource.Token));
        }

        [Fact]
        public async Task StartNegotiationAsync_Sends_StartTls()
        {            
            await negotiator.StartNegotiationAsync(startTls, tokenSource.Token);

            A.CallTo(() => messageSender.SendAsync(startTls, tokenSource.Token)).MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Invalid_Response_Received()
        {
            await negotiator.StartNegotiationAsync(startTls, tokenSource.Token);
            var response = new XmlElement("invalid-element");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(response, tokenSource.Token));
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Tls_Failure_Received()
        {
            var tlsFailure = new XmlElement("failure").Xmlns(XmppNamespaces.Tls);
            await negotiator.StartNegotiationAsync(startTls, tokenSource.Token);

            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(tlsFailure, tokenSource.Token));
        }

        [Fact]
        public async Task HandleAsync_Sets_Tls_Stream()
        {
            var tlsProceed = new XmlElement("proceed").Xmlns(XmppNamespaces.Tls);
            await negotiator.StartNegotiationAsync(startTls, tokenSource.Token);

            await negotiator.HandleAsync(tlsProceed, tokenSource.Token);

            A.CallTo(() => transportCommandHandler.Invoke(TransportCommand.SetTransportLevelSecurity, tokenSource.Token)).MustHaveHappened();
        }
    }
}
