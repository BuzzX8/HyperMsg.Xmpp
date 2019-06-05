using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class SessionNegotiatorTests
    {
        private SessionNegotiator negotiator = new SessionNegotiator();
        private XmlTransceiverFake transceiver = new XmlTransceiverFake();

        private readonly XmlElement sessionFeature = new XmlElement("session").Xmlns(XmppNamespaces.Session);
        private readonly CancellationToken cancellationToken = default;
        private readonly TimeSpan waitTime = TimeSpan.FromSeconds(1);

        [Fact]
        public void NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var invalidFeature = new XmlElement("invalid-feature");

            Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transceiver, invalidFeature, cancellationToken));
        }

        [Fact]
        public void NegotiateAsync_Sends_Correct_Request()
        {
            var task = negotiator.NegotiateAsync(transceiver, sessionFeature, cancellationToken);
            transceiver.WaitSendCompleted(waitTime);

            var request = transceiver.Requests.Single();

            VerifySessionRequest(request);
        }

        private void VerifySessionRequest(XmlElement request)
        {
            Assert.Equal(request.Name, ("iq"));
            var session = request.Children.Single();
            Assert.Equal(session.Name, ("session"));
            Assert.Equal(session.Xmlns(), (XmppNamespaces.Session));
        }

        [Fact]
        public async Task NegotiateAsync_Returns_Correct_Result()
        {
            transceiver.AddResponse(Iq.Result().NewId());

            var result = await negotiator.NegotiateAsync(transceiver, sessionFeature, cancellationToken);

            Assert.False(result);
        }

        [Fact]
        public void NegotiateAsync_Throws_Exception_If_Error_Received()
        {
            transceiver.AddResponse(Iq.Error().Children(new XmlElement("error")));

            Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transceiver, sessionFeature, cancellationToken));
        }
    }
}
