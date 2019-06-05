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
        private readonly IHandler handler;
        private readonly TlsNegotiator negotiator;
        private readonly XmlTransceiverFake transciever;

        private readonly CancellationToken cancellationToken = default;
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(1);

        public TlsNegotiatorTests()
        {
            handler = A.Fake<IHandler>();
            negotiator = new TlsNegotiator(handler);
            transciever = new XmlTransceiverFake();
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transciever, feature, cancellationToken));
        }

        [Fact]
        public void NegotiateAsync_Sends_StartTls()
        {            
            var task = negotiator.NegotiateAsync(transciever, Tls.Start, cancellationToken);
            transciever.WaitSendCompleted(waitTimeout);

            var actualElement = transciever.Requests.Single();

            Assert.Equal(actualElement, Tls.Start);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Response_Received()
        {
            transciever.AddResponse(new XmlElement("invalid-element"));

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transciever, Tls.Start, cancellationToken));
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Failure_Received()
        {
            transciever.AddResponse(Tls.Failure);

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transciever, Tls.Start, cancellationToken));
        }

        [Fact]
        public async Task NegotiateAsync_Sets_Tls_Stream()
        {
            transciever.AddResponse(Tls.Proceed);

            var result = await negotiator.NegotiateAsync(transciever, Tls.Start, cancellationToken);

            Assert.True(result);
            A.CallTo(() => handler.HandleAsync(TransportCommands.SetTransportLevelSecurity, cancellationToken)).MustHaveHappened();
        }
    }
}
