using FakeItEasy;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class BindNegotiatorTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly BindNegotiator negotiator;

        private readonly XmlElement bindFeature = new XmlElement("bind").Xmlns(XmppNamespaces.Bind);
        private readonly CancellationToken cancellationToken = default;
        private readonly TimeSpan waitTime = TimeSpan.FromSeconds(1);
        private readonly string resource = Guid.NewGuid().ToString();

        public BindNegotiatorTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            negotiator = new BindNegotiator(messageSender, resource);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.StartNegotiationAsync(feature, cancellationToken));
        }

        //[Fact]
        //public void NegotiateAsync_Sends_Empty_Bind_If_No_Resource_Provided()
        //{
        //    var negotiator = new BindNegotiator(string.Empty);
        //    var task = negotiator.NegotiateAsync(transceiver, bindFeature, cancellationToken);
        //    transceiver.WaitSendCompleted(waitTime);

        //    var bindRequest = transceiver.Requests.Single();

        //    VerifyBindRequest(bindRequest, string.Empty);
        //}

        [Fact]
        public async Task NegotiateAsync_Sends_Non_Empty_Bind_If_Resource_Provided()
        {
            var actualStanza = default(XmlElement);
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, cancellationToken)).Invokes(foc => actualStanza = foc.GetArgument<XmlElement>(0));

            await negotiator.StartNegotiationAsync(bindFeature, cancellationToken);

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
        public async Task Handle_Throws_Exception_If_Receives_Invalid_Bind_Response()
        {
            await negotiator.StartNegotiationAsync(bindFeature, cancellationToken);
            var response = new XmlElement("invalid-response");

            //Assert.Throws<XmppException>(() => negotiator.HandleAsync(response, default));
        }

        [Fact]
        public async Task Handle_Throws_Exception_If_Bind_Error_Received()
        {
            await negotiator.StartNegotiationAsync(bindFeature, cancellationToken);
            var response = IqStanza.Error().Children(new XmlElement("error"));

            //Assert.Throws<XmppException>(() => negotiator.Handle(response));
        }

        private XmlElement CreateBindResponse(string resource, string jid = "user@domain")
        {
            return IqStanza.Result().Children(
                new XmlElement("bind").Xmlns(XmppNamespaces.Bind).Children(
                    new XmlElement("jid").Value($"{jid}/{resource}")));
        }
    }
}
