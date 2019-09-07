using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Components
{
    public class BindFeatureComponentTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly BindFeatureComponent component;

        private readonly XmlElement bindFeature = new XmlElement("bind").Xmlns(XmppNamespaces.Bind);
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly string resource = Guid.NewGuid().ToString();
        private readonly List<XmlElement> requests = new List<XmlElement>();

        public BindFeatureComponentTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var request = foc.GetArgument<XmlElement>(0);
                requests.Add(request);
            });
            component = new BindFeatureComponent(messageSender, resource);
        }

        [Fact]
        public async Task StartNegotiationAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => component.StartNegotiationAsync(feature, tokenSource.Token));
        }

        [Fact]
        public async Task StartNegotiationAsync_Sends_Empty_Bind_If_No_Resource_Provided()
        {
            var negotiator = new BindFeatureComponent(messageSender, string.Empty);

            await negotiator.StartNegotiationAsync(bindFeature, tokenSource.Token);

            var bindRequest = requests.Single();
            VerifyBindRequest(bindRequest, string.Empty);
        }

        [Fact]
        public async Task StartNegotiationAsync_Sends_Non_Empty_Bind_If_Resource_Provided()
        {
            var actualStanza = default(XmlElement);
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, tokenSource.Token)).Invokes(foc => actualStanza = foc.GetArgument<XmlElement>(0));

            await component.StartNegotiationAsync(bindFeature, tokenSource.Token);

            VerifyBindRequest(actualStanza, resource);
        }

        private void VerifyBindRequest(XmlElement bindRequest, string resource)
        {
            Assert.Equal("iq", bindRequest.Name);
            Assert.Equal("set", bindRequest["type"]);
            var bind = bindRequest.Children.Single();
            Assert.Equal("bind", bind.Name);
            Assert.Equal(XmppNamespaces.Bind, bind.Xmlns());

            if (!string.IsNullOrEmpty(resource))
            {
                var resourceElement = bind.Children.Single();
                Assert.Equal("resource", resourceElement.Name);
            }
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Receives_Invalid_Bind_Response()
        {
            await component.StartNegotiationAsync(bindFeature, tokenSource.Token);
            var response = new XmlElement("invalid-response");

            await Assert.ThrowsAsync<XmppException>(() => component.HandleAsync(response, tokenSource.Token));
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Bind_Error_Received()
        {
            await component.StartNegotiationAsync(bindFeature, tokenSource.Token);
            var response = IqStanza.Error().Children(new XmlElement("error"));

            await Assert.ThrowsAsync<XmppException>(() => component.HandleAsync(response, tokenSource.Token));
        }

        [Fact]
        public async Task HandleAsync_Returns_Completed_NegotiationResult_If_Corrent_Bind_Response_Received()
        {
            await component.StartNegotiationAsync(bindFeature, tokenSource.Token);
            var response = CreateBindResponse(resource);

            var status = await component.HandleAsync(response, tokenSource.Token);

            Assert.Equal(FeatureNegotiationState.Completed, status);
        }

        [Fact]
        public async Task HandleAsync_Rises_JidBound_If_Correct_Response_Received()
        {
            var expectedJid = (Jid)$"expected@jid.com";
            var actualJid = default(Jid);
            component.JidBound += jid => actualJid = jid;
            await component.StartNegotiationAsync(bindFeature, tokenSource.Token);
            var response = CreateBindResponse(string.Empty, expectedJid);

            await component.HandleAsync(response, tokenSource.Token);

            Assert.Equal(expectedJid, actualJid);
        }

        private XmlElement CreateBindResponse(string resource, string jid = "user@domain")
        {
            return IqStanza.Result().Children(
                new XmlElement("bind").Xmlns(XmppNamespaces.Bind).Children(
                    new XmlElement("jid").Value($"{jid}/{resource}")));
        }
    }
}
