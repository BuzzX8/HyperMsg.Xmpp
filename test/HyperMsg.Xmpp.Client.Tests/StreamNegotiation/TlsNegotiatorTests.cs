using FakeItEasy;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class TlsNegotiatorTests
    {
        private readonly XmlElement startTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly AsyncAction<TransportCommand> transportCommandHandler;
        private readonly TlsNegotiator negotiator;

        private readonly CancellationToken cancellationToken = default;
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(1);

        public TlsNegotiatorTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            transportCommandHandler = A.Fake<AsyncAction<TransportCommand>>();
            negotiator = new TlsNegotiator(transportCommandHandler, messageSender);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(feature, cancellationToken));
        }

        [Fact]
        public async Task NegotiateAsync_Sends_StartTls()
        {            
            await negotiator.NegotiateAsync(startTls, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(startTls, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task Handle_Throws_Exception_If_Invalid_Response_Received()
        {
            await negotiator.NegotiateAsync(startTls, cancellationToken);
            var response = new XmlElement("invalid-element");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(response, cancellationToken));
        }

        [Fact]
        public async Task Handle_Throws_Exception_If_Tls_Failure_Received()
        {
            var tlsFailure = new XmlElement("failure").Xmlns(XmppNamespaces.Tls);
            await negotiator.NegotiateAsync(startTls, cancellationToken);

            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(tlsFailure, cancellationToken));
        }

        [Fact]
        public async Task NegotiateAsync_Sets_Tls_Stream()
        {
            var tlsProceed = new XmlElement("proceed").Xmlns(XmppNamespaces.Tls);
            await negotiator.NegotiateAsync(startTls, cancellationToken);

            await negotiator.HandleAsync(tlsProceed, cancellationToken);

            A.CallTo(() => transportCommandHandler.Invoke(TransportCommand.SetTransportLevelSecurity, cancellationToken)).MustHaveHappened();
        }
    }
}
