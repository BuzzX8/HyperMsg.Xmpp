using FakeItEasy;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Components
{
    public class SessionFeatureComponentTests
    {
        private readonly IMessageSender messageSender;
        private readonly SessionFeatureComponent component;

        private readonly XmlElement sessionFeature = new XmlElement("session").Xmlns(XmppNamespaces.Session);
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly List<XmlElement> requests = new List<XmlElement>();

        public SessionFeatureComponentTests()
        {
            messageSender = A.Fake<IMessageSender>();
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var request = foc.GetArgument<XmlElement>(0);
                requests.Add(request);
            });
            component = new SessionFeatureComponent(messageSender);
        }

        [Fact]
        public async Task StartNegotiationAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var invalidFeature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => component.StartNegotiationAsync(invalidFeature, tokenSource.Token));
        }

        [Fact]
        public async Task StartNegotiationAsync_Sends_Correct_Request()
        {
            await component.StartNegotiationAsync(sessionFeature, tokenSource.Token);

            var request = requests.Single();

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
        public async Task HandleAsync_Returns_Correct_NegotiationStatus()
        {
            await component.StartNegotiationAsync(sessionFeature, tokenSource.Token);
            var response = IqStanza.Result().NewId();

            var status = await component.HandleAsync(response, tokenSource.Token);

            Assert.Equal(FeatureNegotiationState.Completed, status);
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Error_Received()
        {
            await component.StartNegotiationAsync(sessionFeature, tokenSource.Token);
            var response = IqStanza.Error().Children(new XmlElement("error"));

            await Assert.ThrowsAsync<XmppException>(() => component.HandleAsync(response, tokenSource.Token));
        }
    }
}
