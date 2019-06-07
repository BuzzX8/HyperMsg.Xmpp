using FakeItEasy;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class ClientStreamNegotiatorTests
    {
        private ClientStreamNegotiator negotiator;
        private XmlTransceiverFake transceiver;
        private XmppConnectionSettings settings;

        private readonly TimeSpan waitTime = TimeSpan.FromSeconds(2);
        private readonly Jid jid = "user@domain.com";

        public ClientStreamNegotiatorTests()
        {            
            transceiver = new XmlTransceiverFake();
            settings = new XmppConnectionSettings(jid);
            negotiator = new ClientStreamNegotiator(Enumerable.Empty<IFeatureNegotiator>(), transceiver, settings);
        }

        [Fact]
        public void NegotiateAsync_Sends_StreamHeader()
        {
            var task = negotiator.HandleAsync(TransportMessage.Opened, CancellationToken.None);
            transceiver.WaitSendCompleted(waitTime);

            var header = transceiver.Requests.Single();

            VerifyStreamHeader(header);
        }

        private void VerifyStreamHeader(XmlElement element)
        {
            Assert.Equal(jid.Domain, element["to"]);
            Assert.Equal("stream:stream", element.Name);
            Assert.Equal(XmppNamespaces.JabberClient, element["xmlns"]);
            Assert.Equal(XmppNamespaces.Streams, element["xmlns:stream"]);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Header_Received()
        {
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);
            transceiver.AddResponse(incorrectHeader);
            var task = negotiator.HandleAsync(TransportMessage.Opened, CancellationToken.None);
            transceiver.WaitSendCompleted(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Incorrect_Features_Received()
        {
            AddStreamHeaderResponse();
            transceiver.AddResponse(new XmlElement("stream:incorrect-features"));
            var task = negotiator.HandleAsync(TransportMessage.Opened, CancellationToken.None);
            transceiver.WaitSendCompleted(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        private void AddStreamHeaderResponse()
        {
            var header = StreamHeader.Server().From(jid.Domain);
            transceiver.AddResponse(header);
        }

        private void AddFeaturesResponse(params string[] features)
        {
            var element = new XmlElement("stream:features");

            foreach (var feature in features)
            {
                element.Children(new XmlElement(feature));
            }

            transceiver.AddResponse(element);
        }
    }
}
