using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class ConnectionComponentTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly XmppConnectionSettings settings;
        private readonly ConnectionComponent negotiator;

        private readonly List<XmlElement> sentElements;
        private readonly Jid jid = "user@domain.com";        

        public ConnectionComponentTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            settings = new XmppConnectionSettings(jid);
            negotiator = new ConnectionComponent(messageSender, settings);
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
            OpenTransportAsync().Wait();

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
            await OpenTransportAsync();
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);

            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(incorrectHeader, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_Transits_To_WaitingFeatures_State()
        {
            await OpenTransportAsync();
            var streamHeader = CreateStreamHeaderResponse();            

            await negotiator.HandleAsync(streamHeader, CancellationToken.None);

            Assert.Equal(StreamNegotiationState.WaitingStreamFeatures, negotiator.State);
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Incorrect_Features_Received()
        {
            await SetWaitingFeaturesStateAsync(CancellationToken.None);

            var features = new XmlElement("stream:incorrect-features");
            await Assert.ThrowsAsync<XmppException>(() => negotiator.HandleAsync(features, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_Transits_To_Done_If_Empty_FeaturesResponse_Received()
        {
            await SetWaitingFeaturesStateAsync();
            var features = CreateFeaturesResponse();

            await negotiator.HandleAsync(features, default);

            Assert.Equal(StreamNegotiationState.Done, negotiator.State);
        }

        [Fact]
        public async Task HandleAsync_Invokes_FeatureNegotiator()
        {
            await SetWaitingFeaturesStateAsync();
            var cancellationToken = new CancellationToken();
            var featureName = Guid.NewGuid().ToString();
            var featureNegotiator = A.Fake<FeatureMessageHandler>();
            negotiator.AddFeatureHandler(featureName, featureNegotiator);

            var featuresResponse = CreateFeaturesResponse(new[] { featureName });
            await negotiator.HandleAsync(featuresResponse, cancellationToken);

            A.CallTo(() => featureNegotiator.Invoke(featuresResponse.Child(featureName), cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_Transits_To_Negotiatin_Features_State()
        {
            await SetWaitingFeaturesStateAsync();
            var featureName = Guid.NewGuid().ToString();
            var featureNegotiator = A.Fake<FeatureMessageHandler>();
            negotiator.AddFeatureHandler(featureName, featureNegotiator);

            await ReceiveFeaturesAsync(new[] { featureName });

            Assert.Equal(StreamNegotiationState.NegotiatingFeature, negotiator.State);
        }

        [Fact]
        public async Task HandleAsync_Transits_To_WaitingStreamFeatures_State_If_Feature_Negotiator_Returns_Completed()
        {
            var featureName = Guid.NewGuid().ToString();
            negotiator.AddFeatureHandler(featureName, (m, t) => Task.FromResult(FeatureNegotiationState.Completed));
            await SetNegotiatingFeatureStateAsync(featureName);

            await negotiator.HandleAsync(new XmlElement("message"), default);

            Assert.Equal(StreamNegotiationState.WaitingStreamFeatures, negotiator.State);
        }

        [Fact]
        public async Task HandleAsync_Does_Not_Changes_State_If_Feature_Negotiator_Returns_Negotiating()
        {
            var featureName = Guid.NewGuid().ToString();
            negotiator.AddFeatureHandler(featureName, (m, t) => Task.FromResult(FeatureNegotiationState.Negotiating));
            await SetNegotiatingFeatureStateAsync(featureName);

            await negotiator.HandleAsync(new XmlElement("message"), default);

            Assert.Equal(StreamNegotiationState.NegotiatingFeature, negotiator.State);
        }

        [Fact]
        public async Task HandleAsync_Sends_StreamHeader_If_Feature_Negotiator_Returns_StreamRestartRequired()
        {
            var featureName = Guid.NewGuid().ToString();
            negotiator.AddFeatureHandler(featureName, (m, t) => Task.FromResult(FeatureNegotiationState.StreamRestartRequired));
            await SetNegotiatingFeatureStateAsync(featureName);
            sentElements.Clear();

            await negotiator.HandleAsync(new XmlElement("message"), default);
            var request = sentElements.Single();

            VerifyStreamHeader(request);
        }

        [Fact]
        public async Task HandleAsync_Transits_To__If_Feature_Negotiator_Returns_StreamRestartRequired()
        {
            var featureName = Guid.NewGuid().ToString();
            negotiator.AddFeatureHandler(featureName, (m, t) => Task.FromResult(FeatureNegotiationState.StreamRestartRequired));
            await SetNegotiatingFeatureStateAsync(featureName);
            
            await negotiator.HandleAsync(new XmlElement("message"), default);

            Assert.Equal(StreamNegotiationState.WaitingStreamHeader, negotiator.State);
        }

        private XmlElement CreateStreamHeaderResponse() => StreamHeader.Server().From(jid.Domain);

        private Task OpenTransportAsync() => negotiator.HandleTransportEventAsync(new TransportEventArgs(TransportEvent.Opened), default);

        private async Task SetWaitingFeaturesStateAsync(CancellationToken cancellationToken = default)
        {
            await OpenTransportAsync();
            var streamHeader = CreateStreamHeaderResponse();
            await negotiator.HandleAsync(streamHeader, cancellationToken);
        }
                
        private async Task SetNegotiatingFeatureStateAsync(string featureName)
        {
            await SetWaitingFeaturesStateAsync();
            await ReceiveFeaturesAsync(new[] { featureName });            
        }

        private XmlElement CreateFeaturesResponse(params string[] features)
        {
            var element = new XmlElement("stream:features");

            foreach (var feature in features)
            {
                element.Children(new XmlElement(feature));
            }

            return element;
        }

        private Task ReceiveFeaturesAsync(string[] features, CancellationToken cancellationToken = default)
        {
            var response = CreateFeaturesResponse(features);
            return negotiator.HandleAsync(response, cancellationToken);
        }
    }
}
