using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class BindNegotiatorTests
    {
        private BindNegotiator negotiator;
        private ITransceiver<XmlElement, XmlElement> channel;
        private XmlElement bindFeature = new XmlElement("bind").Xmlns(XmppNamespaces.Bind);
        private string resource = Guid.NewGuid().ToString();
        private TimeSpan waitTime = TimeSpan.FromSeconds(1);

        public BindNegotiatorTests()
        {
            negotiator = new BindNegotiator();
            channel = A.Fake<ITransceiver<XmlElement, XmlElement>>();
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, feature));
        }

        [Fact]
        public void NegotiateAsync_Sends_Empty_Bind_If_No_Resource_Provided()
        {
            //channel.IsManualSync = true;
            var task = negotiator.NegotiateAsync(channel, bindFeature);
            //channel.WaitForSend(waitTime);

            var bindRequest = default(XmlElement);// channel.SentElements.Single();

            VerifyBindRequest(bindRequest);
        }

        [Fact]
        public void NegotiateAsync_Sends_Non_Empty_Bind_If_Resource_Provided()
        {
            //channel.IsManualSync = true;
            negotiator = new BindNegotiator(resource);
            var task = negotiator.NegotiateAsync(channel, bindFeature);
            //channel.WaitForSend(waitTime);

            var bindRequest = default(XmlElement);// channel.SentElements.Single();

            VerifyBindRequest(bindRequest, resource);
        }

        private void VerifyBindRequest(XmlElement bindRequest, string resource = "")
        {
            Assert.Equal(bindRequest.Name, "iq");
            Assert.Equal(bindRequest["type"], "set");
            var bind = default(XmlElement);// bindRequest.Children.Single();
            Assert.Equal(bind.Name, "bind");
            Assert.Equal(bind.Xmlns(), XmppNamespaces.Bind);

            if (!string.IsNullOrEmpty(resource))
            {
                var resourceElement = default(XmlElement); //bind.Children.Single();
                Assert.Equal(resourceElement.Name, "resource");
            }
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Receives_Invalid_Bind_Response()
        {
            //channel.EnqueueResponse(new XmlElement("invalid-response"));

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, bindFeature));
        }

        [Fact]
        public void NegotiateAsync_Throws_Exception_If_Bind_Error_Received()
        {
            //channel.EnqueueResponse(
            //    Iq.Error()
            //    .Children(new XmlElement("error")));

            Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, bindFeature));
        }

        [Fact]
        public async Task NegotiateAsync_Returns_Correct_Result()
        {
            //channel.EnqueueResponse(CreateBindResponse(resource));

            var result = await negotiator.NegotiateAsync(channel, bindFeature);

            Assert.False(result);
        }

        [Fact]
        public async Task NegotiateAsync_Returns_Result_With_Bound_Jid()
        {
            var jid = "user@domain";
            //channel.EnqueueResponse(CreateBindResponse(resource, jid));

            var result = await negotiator.NegotiateAsync(channel, bindFeature);

            //Assert.Equal(result.Data[ResultData.BoundJid], Jid.Parse(jid + '/' + resource));
        }

        private XmlElement CreateBindResponse(string resource, string jid = "user@domain")
        {
            return Iq.Result().Children(
                new XmlElement("bind").Xmlns(XmppNamespaces.Bind).Children(
                    new XmlElement("jid").Value($"{jid}/{resource}")));
        }
    }
}
