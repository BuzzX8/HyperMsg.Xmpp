using HyperMsg.Extensions;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.FeatureNegotiators;
using HyperMsg.Xmpp.Xml;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Tests.FeatureNegotiators
{
    public class SessionNegotiatorTests
    {
        private readonly XmlElement sessionFeature = new XmlElement("session").Xmlns(XmppNamespaces.Session);

        private readonly SessionNegotiator negotiator;
        private readonly CancellationTokenSource tokenSource;

        private readonly IMessagingContext messagingContext;

        [Fact]
        public async Task StartNegotiationAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var invalidFeature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(messagingContext, invalidFeature, tokenSource.Token));
        }

        [Fact]
        public async Task StartNegotiationAsync_Sends_Correct_Request()
        {
            var request = default(XmlElement);
            messagingContext.Observable.OnTransmit<XmlElement>(e => request = e);
            await negotiator.NegotiateAsync(messagingContext, sessionFeature, tokenSource.Token);

            VerifySessionRequest(request);
        }

        private void VerifySessionRequest(XmlElement request)
        {
            Assert.Equal("iq", request.Name);
            var session = request.Children.Single();
            Assert.Equal("session", session.Name);
            Assert.Equal(session.Xmlns(), XmppNamespaces.Session);
        }

        [Fact]
        public async Task Completes_Task_If_Response_Received()
        {
            var task = await negotiator.NegotiateAsync(messagingContext, sessionFeature, tokenSource.Token);
            var response = IqStanza.Result().NewId();

            messagingContext.Sender.Received(response);

            Assert.True(task.IsCompleted);
        }

        [Fact]
        public async Task Throws_Exception_If_Error_Received()
        {
            await negotiator.NegotiateAsync(messagingContext, sessionFeature, tokenSource.Token);
            var response = IqStanza.Error().Children(new XmlElement("error"));

            await Assert.ThrowsAsync<XmppException>(() => messagingContext.Sender.ReceivedAsync(response, tokenSource.Token));
        }
    }
}
