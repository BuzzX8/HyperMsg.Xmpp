using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class SessionNegotiatorTests
    {
        private SessionNegotiator negotiator;
        private ITransceiver<XmlElement, XmlElement> channel;

        private XmlElement sessionFeature = new XmlElement("session").Xmlns(XmppNamespaces.Session);
        private TimeSpan waitTime = TimeSpan.FromSeconds(1);

        public SessionNegotiatorTests()
        {
            negotiator = new SessionNegotiator();
            channel = null;
        }

        [Fact]
        public void NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var invalidFeature = new XmlElement("invalid-feature");

            Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, invalidFeature));
        }

        [Fact]
        public void NegotiateAsync_Sends_Correct_Request()
        {
            //channel.IsManualSync = true;
            var task = negotiator.NegotiateAsync(channel, sessionFeature);
            //channel.WaitForSend(waitTime);

            var request = default(XmlElement);// channel.SentElements.Single();

            VerifySessionRequest(request);
        }

        private void VerifySessionRequest(XmlElement request)
        {
            Assert.Equal(request.Name, ("iq"));
            var session = default(XmlElement);// request.Children.Single();
            Assert.Equal(session.Name, ("session"));
            Assert.Equal(session.Xmlns(), (XmppNamespaces.Session));
        }

        [Fact]
        public async Task NegotiateAsync_Returns_correct_Result()
        {
            //channel.EnqueueResponse(Iq.Result().NewId());

            var result = await negotiator.NegotiateAsync(channel, sessionFeature);

            Assert.False(result);
        }

        [Fact]
        public void NegotiateAsync_Throws_Exception_If_Error_Received()
        {
            //channel.EnqueueResponse(Iq.Error().Children(new XmlElement("error")));

            Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, sessionFeature));
        }
    }
}
