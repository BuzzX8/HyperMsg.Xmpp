using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class ClientStreamNegotiatorTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly XmppConnectionSettings settings;
        private readonly ClientStreamNegotiator negotiator;

        private readonly List<XmlElement> sentElements;
        private readonly Jid jid = "user@domain.com";        

        public ClientStreamNegotiatorTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            settings = new XmppConnectionSettings(jid);
            negotiator = new ClientStreamNegotiator(messageSender, settings);
            sentElements = new List<XmlElement>();
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var message = foc.GetArgument<XmlElement>(0);
                sentElements.Add(message);
            }).Returns(Task.CompletedTask);
        }

        [Fact]
        public void HandleTransportEvent_Sends_StreamHeader()
        {
            Assert.Equal(StreamNegotiationState.None, negotiator.State);
            OpenTransport();

            Assert.Equal(StreamNegotiationState.WaitingStreamHeader, negotiator.State);
            VerifyStreamHeader(sentElements.Single());
        }

        private void VerifyStreamHeader(XmlElement element)
        {
            Assert.Equal(jid.Domain, element["to"]);
            Assert.Equal("stream:stream", element.Name);
            Assert.Equal(XmppNamespaces.JabberClient, element["xmlns"]);
            Assert.Equal(XmppNamespaces.Streams, element["xmlns:stream"]);
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Invalid_Header_Received()
        {
            OpenTransport();
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);

            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(incorrectHeader, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_Transits_To_WaitingFeatures_State()
        {
            OpenTransport();
            var streamHeader = CreateStreamHeaderResponse();            

            await negotiator.HandleAsync(streamHeader, CancellationToken.None);

            Assert.Equal(StreamNegotiationState.WaitingFeatures, negotiator.State);
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Incorrect_Features_Received()
        {
            OpenTransport();
            var streamHeader = CreateStreamHeaderResponse();
            var features = new XmlElement("stream:incorrect-features");            
            await negotiator.HandleAsync(streamHeader, CancellationToken.None);
            
            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(features, CancellationToken.None));
        }

        private XmlElement CreateStreamHeaderResponse() => StreamHeader.Server().From(jid.Domain);

        private void OpenTransport() => negotiator.HandleTransportEvent(null, new TransportEventArgs(TransportEvent.Opened));

        private void AddFeaturesResponse(params string[] features)
        {
            var element = new XmlElement("stream:features");

            foreach (var feature in features)
            {
                element.Children(new XmlElement(feature));
            }

            //transceiver.AddResponse(element);
        }
    }
}
