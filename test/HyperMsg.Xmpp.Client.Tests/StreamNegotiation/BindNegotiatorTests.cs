using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class BindNegotiatorTests
    {
        private readonly BindNegotiator negotiator;
        private readonly XmlTransceiverFake transceiver;

        private readonly XmlElement bindFeature = new XmlElement("bind").Xmlns(XmppNamespaces.Bind);
        private readonly CancellationToken cancellationToken = default;
        private readonly TimeSpan waitTime = TimeSpan.FromSeconds(1);
        private readonly string resource = Guid.NewGuid().ToString();

        public BindNegotiatorTests()
        {            
            negotiator = new BindNegotiator(resource);
            transceiver = new XmlTransceiverFake();
        }

        //[Fact]
        //public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        //{
        //    var feature = new XmlElement("invalid-feature");

        //    await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transceiver, feature, cancellationToken));
        //}

        //[Fact]
        //public void NegotiateAsync_Sends_Empty_Bind_If_No_Resource_Provided()
        //{
        //    var negotiator = new BindNegotiator(string.Empty);
        //    var task = negotiator.NegotiateAsync(transceiver, bindFeature, cancellationToken);
        //    transceiver.WaitSendCompleted(waitTime);

        //    var bindRequest = transceiver.Requests.Single();

        //    VerifyBindRequest(bindRequest, string.Empty);
        //}

        //[Fact]
        //public void NegotiateAsync_Sends_Non_Empty_Bind_If_Resource_Provided()
        //{
        //    var task = negotiator.NegotiateAsync(transceiver, bindFeature, cancellationToken);
        //    transceiver.WaitSendCompleted(waitTime);

        //    var bindRequest = transceiver.Requests.Single();

        //    VerifyBindRequest(bindRequest, resource);
        //}

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

        //[Fact]
        //public async Task NegotiateAsync_Throws_Exception_If_Receives_Invalid_Bind_Response()
        //{
        //    transceiver.AddResponse(new XmlElement("invalid-response"));

        //    await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transceiver, bindFeature, cancellationToken));
        //}

        //[Fact]
        //public void NegotiateAsync_Throws_Exception_If_Bind_Error_Received()
        //{
        //    transceiver.AddResponse(
        //        Iq.Error().Children(new XmlElement("error")));

        //    Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(transceiver, bindFeature, cancellationToken));
        //}

        //[Fact]
        //public async Task NegotiateAsync_Returns_Correct_Result()
        //{
        //    transceiver.AddResponse(CreateBindResponse(resource));

        //    var result = await negotiator.NegotiateAsync(transceiver, bindFeature, cancellationToken);

        //    Assert.False(result);
        //}

        private XmlElement CreateBindResponse(string resource, string jid = "user@domain")
        {
            return Iq.Result().Children(
                new XmlElement("bind").Xmlns(XmppNamespaces.Bind).Children(
                    new XmlElement("jid").Value($"{jid}/{resource}")));
        }
    }
}
