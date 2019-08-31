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
        private readonly ConnectionComponent component;

        private readonly CancellationTokenSource tokenSource;
        private readonly List<XmlElement> sentElements;
        private readonly Jid jid = "user@domain.com";        

        public ConnectionComponentTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            settings = new XmppConnectionSettings(jid);
            component = new ConnectionComponent(messageSender, settings);
            sentElements = new List<XmlElement>();
            tokenSource = new CancellationTokenSource();
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var message = foc.GetArgument<XmlElement>(0);
                sentElements.Add(message);
            }).Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task OpenStreamAsync_Sends_StreamHeader()
        {            
            await component.OpenStreamAsync(tokenSource.Token);

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
            await component.OpenStreamAsync(tokenSource.Token);
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);

            await Assert.ThrowsAsync<XmppException>(() => component.HandleAsync(incorrectHeader, tokenSource.Token));
        }

        [Fact]
        public async Task HandleAsync_Returns_Correct_Negotiation_State_For_Correct_Stream_Header_Response()
        {
            await component.OpenStreamAsync(tokenSource.Token);
            var streamHeader = CreateStreamHeaderResponse();            

            var state = await component.HandleAsync(streamHeader, tokenSource.Token);

            Assert.Equal(StreamNegotiationState.WaitingStreamFeatures, state);
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_Incorrect_Features_Response_Received()
        {
            await SetWaitingFeaturesStateAsync();

            var features = new XmlElement("stream:incorrect-features");
            await Assert.ThrowsAsync<XmppException>(() => component.HandleAsync(features, tokenSource.Token));
        }

        [Fact]
        public async Task HandleAsync_Returns_Done_State_For_Empty_Features_Response()
        {
            await SetWaitingFeaturesStateAsync();
            var features = CreateFeaturesResponse();

            var state = await component.HandleAsync(features, tokenSource.Token);

            Assert.Equal(StreamNegotiationState.Done, state);
        }

        [Fact]
        public async Task HandleAsync_Invokes_FeatureComponent_StartNegotiationAsync_Which_Can_Negotiate_Feature()
        {
            var featureName = Guid.NewGuid().ToString();
            var featuresResponse = CreateFeaturesResponse(new[] { featureName });
            var featureComponent = A.Fake<IFeatureComponent>();
            A.CallTo(() => featureComponent.CanNegotiate(featuresResponse.Child(featureName))).Returns(true);

            component.FeatureComponents.Add(featureComponent);
            await SetWaitingFeaturesStateAsync();

            await component.HandleAsync(featuresResponse, tokenSource.Token);

            A.CallTo(() => featureComponent.StartNegotiationAsync(featuresResponse.Child(featureName), tokenSource.Token)).MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_Returns_NegotiatingFeature_State_For_Non_Empty_Features_Response()
        {
            await SetWaitingFeaturesStateAsync();
            CreateAndAddFakeNegotiator(FeatureNegotiationState.Completed);

            var state = await ReceiveFeaturesAsync(new[] { Guid.NewGuid().ToString() });

            Assert.Equal(StreamNegotiationState.NegotiatingFeature, state);
        }

        [Fact]
        public async Task HandleAsync_Returns_WaitingStreamFeatures_State_If_FeatureNegotiator_HandleAsync_Returns_Completed()
        {
            var featureName = Guid.NewGuid().ToString();
            CreateAndAddFakeNegotiator(FeatureNegotiationState.Completed);
            await SetNegotiatingFeatureStateAsync(featureName);

            var state = await component.HandleAsync(new XmlElement("message"), tokenSource.Token);

            Assert.Equal(StreamNegotiationState.WaitingStreamFeatures, state);
        }

        [Fact]
        public async Task HandleAsync_Returns_NegotiatingFeatures_State_If_FeatureNegotiator_HandleAsync_Returns_Nogotiating()
        {
            var featureName = Guid.NewGuid().ToString();
            CreateAndAddFakeNegotiator(FeatureNegotiationState.Negotiating);

            var state = await SetNegotiatingFeatureStateAsync(featureName);

            Assert.Equal(StreamNegotiationState.NegotiatingFeature, state);
        }

        [Fact]
        public async Task HandleAsync_Sends_StreamHeader_If_Feature_Negotiator_Returns_StreamRestartRequired()
        {
            var featureName = Guid.NewGuid().ToString();
            CreateAndAddFakeNegotiator(FeatureNegotiationState.StreamRestartRequire);
            await SetNegotiatingFeatureStateAsync(featureName);
            sentElements.Clear();

            await component.HandleAsync(new XmlElement("message"), tokenSource.Token);
            var request = sentElements.Single();

            VerifyStreamHeader(request);
        }

        [Fact]
        public async Task HandleAsync_Returns_WaitingStreamHeader_If_FeatureComponent_Returns_StreamRestartRequired()
        {
            var featureName = Guid.NewGuid().ToString();
            CreateAndAddFakeNegotiator(FeatureNegotiationState.StreamRestartRequire);
            await SetNegotiatingFeatureStateAsync(featureName);           

            var state = await component.HandleAsync(new XmlElement("message"), tokenSource.Token);

            Assert.Equal(StreamNegotiationState.WaitingStreamHeader, state);
        }

        private IFeatureComponent CreateAndAddFakeNegotiator(FeatureNegotiationState handleResult)
        {
            var featureComponent = A.Fake<IFeatureComponent>();
            A.CallTo(() => featureComponent.CanNegotiate(A<XmlElement>._)).Returns(true);
            A.CallTo(() => featureComponent.StartNegotiationAsync(A<XmlElement>._, tokenSource.Token)).Returns(Task.FromResult(FeatureNegotiationState.Negotiating));
            A.CallTo(() => featureComponent.HandleAsync(A<XmlElement>._, tokenSource.Token)).Returns(Task.FromResult(handleResult));
            component.FeatureComponents.Add(featureComponent);

            return featureComponent;
        }

        private XmlElement CreateStreamHeaderResponse() => StreamHeader.Server().From(jid.Domain);

        private async Task SetWaitingFeaturesStateAsync()
        {
            await component.OpenStreamAsync(tokenSource.Token);
            var streamHeader = CreateStreamHeaderResponse();
            await component.HandleAsync(streamHeader, tokenSource.Token);
        }
                
        private async Task<StreamNegotiationState> SetNegotiatingFeatureStateAsync(string featureName)
        {
            await SetWaitingFeaturesStateAsync();
            return await ReceiveFeaturesAsync(new[] { featureName });            
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

        private Task<StreamNegotiationState> ReceiveFeaturesAsync(string[] features)
        {
            var response = CreateFeaturesResponse(features);
            return component.HandleAsync(response, tokenSource.Token);
        }
    }
}
