using FakeItEasy;
using System;
using System.Linq;
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
            negotiator = new ClientStreamNegotiator(Enumerable.Empty<IFeatureNegotiator>());
            transceiver = new XmlTransceiverFake();
            settings = new XmppConnectionSettings(jid);
        }

        [Fact]
        public void Negotiate_Sends_StreamHeader()
        {
            var task = Task.Run(() => negotiator.Negotiate(transceiver, settings));
            transceiver.WaitSendCompleted(waitTime);

            var header = transceiver.Requests.Single();

            VerifyStreamHeader(header);
        }

        [Fact]
        public void NegotiateAsync_Sends_StreamHeader()
        {
            var task = negotiator.NegotiateAsync(transceiver, settings);
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
        public async Task Negotiate_Throws_Exception_If_Invalid_Header_Received()
        {
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);
            transceiver.AddResponse(incorrectHeader);
            var task = Task.Run(() => negotiator.Negotiate(transceiver, settings));
            transceiver.WaitSendCompleted(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Header_Received()
        {
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);
            transceiver.AddResponse(incorrectHeader);
            var task = negotiator.NegotiateAsync(transceiver, settings);
            transceiver.WaitSendCompleted(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task Negotiate_Throws_Exception_If_Incorrect_Features_Received()
        {
            AddStreamHeaderResponse();
            transceiver.AddResponse(new XmlElement("stream:incorrect-features"));
            var task = Task.Run(() => negotiator.Negotiate(transceiver, settings));
            transceiver.WaitSendCompleted(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Incorrect_Features_Received()
        {
            AddStreamHeaderResponse();
            transceiver.AddResponse(new XmlElement("stream:incorrect-features"));
            var task = negotiator.NegotiateAsync(transceiver, settings);
            transceiver.WaitSendCompleted(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public void Negotiate_Does_Not_Negotiates_Features_If_No_Negotiators()
        {
            AddStreamHeaderAndFeaturesResponse();

            var task = Task.Run(() => negotiator.Negotiate(transceiver, settings));

            task.Wait(waitTime);
            Assert.Empty(task.Result.NegotiatedFeatures);
        }

        [Fact]
        public void NegotiateAsync_Does_Not_Negotiates_Features_If_No_Negotiators()
        {
            AddStreamHeaderAndFeaturesResponse();

            var task = negotiator.NegotiateAsync(transceiver, settings);

            task.Wait(waitTime);
            Assert.Empty(task.Result.NegotiatedFeatures);
        }

        [Fact]
        public void Negotiate_Does_Not_Uses_Negotiator_For_Not_Applicable_Feature()
        {
            AddStreamHeaderAndFeaturesResponse("Fact-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator("feature-0", false);

            var result = negotiator.Negotiate(transceiver, settings);

            Assert.Empty(result.NegotiatedFeatures);
            A.CallTo(() => featureNegotiator.Negotiate(A<ITransceiver<XmlElement, XmlElement>>._, A<XmlElement>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task NegotiateAsync_Does_Not_Uses_Negotiator_For_Not_Applicable_Feature()
        {
            AddStreamHeaderAndFeaturesResponse("Fact-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator("feature-0", false);

            var result = await negotiator.NegotiateAsync(transceiver, settings);

            Assert.Empty(result.NegotiatedFeatures);
            A.CallTo(() => featureNegotiator.Negotiate(A<ITransceiver<XmlElement, XmlElement>>._, A<XmlElement>._))
                .MustNotHaveHappened();
        }

        //[Fact]
        public void Negotiate_Uses_Applicable_Feature_Negotiator_Without_Stream_Restart()
        {
            string featureName = "Fact-feature";
            AddStreamHeaderAndFeaturesResponse(featureName);
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, false);

            var result = negotiator.Negotiate(transceiver, settings);

            //Assert.Equal(channel.SentElements.Count, (1));
            //Assert.Contains(featureName, result.NegotiatedFeature);
        }

        //[Fact]
        public async Task NegotiateAsync_Uses_Applicable_Feature_Negotiator_Without_Stream_Restart()
        {
            string featureName = "Fact-feature";
            AddStreamHeaderAndFeaturesResponse(featureName);
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, false);

            var result = await negotiator.NegotiateAsync(transceiver, settings);

            //Assert.Equal(channel.SentElements.Count, (1));
            //Assert.Contains(result.NegotiatedFeatures, featureName);
        }

        //[Fact]
        public void Negotiate_Uses_Applicable_Feature_Negotiator_With_Stream_Restart()
        {
            string featureName = "Fact-feature";
            AddStreamHeaderAndFeaturesResponse(featureName);
            AddStreamHeaderAndFeaturesResponse("some-other-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, true);

            var result = negotiator.Negotiate(transceiver, settings);

            //Assert.Equal(channel.SentElements.Count, (2));
            //Assert.Contains(result.NegotiatedFeatures, featureName);
        }

        //[Fact]
        public async Task NegotiateAsync_Uses_Applicable_Feature_Negotiator_With_Stream_Restart()
        {
            string featureName = "Fact-feature";
            AddStreamHeaderAndFeaturesResponse(featureName);
            AddStreamHeaderAndFeaturesResponse("some-other-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, true);

            var result = await negotiator.NegotiateAsync(transceiver, settings);

            //Assert.Equal(channel.SentElements.Count, (2));
            //Assert.Contains(result.NegotiatedFeatures, featureName);
        }

        //[Fact]
        public void Negotiate_Returns_Negotiation_Data_Received_From_Feature_Negotiator()
        {
            string featureName = "feature-0";
            string dataName = "CustomData";
            object dataValue = Guid.NewGuid();
            var featureResult = new FeatureNegotiationResult(false);
            featureResult.Data[dataName] = dataValue;
            AddStreamHeaderAndFeaturesResponse(featureName);
            CreateAndAddFeatureNegotiator(featureName, featureResult);

            var result = negotiator.Negotiate(transceiver, settings);

            Assert.Equal(result.Data[dataName], (dataValue));
        }

        //[Fact]
        public async Task NegotiateAsync_Returns_Negotiation_Data_Received_From_Feature_Negotiator()
        {
            string featureName = "feature-0";
            string dataName = "CustomData";
            object dataValue = Guid.NewGuid();
            var featureResult = new FeatureNegotiationResult(false);
            featureResult.Data[dataName] = dataValue;
            AddStreamHeaderAndFeaturesResponse(featureName);
            CreateAndAddFeatureNegotiator(featureName, featureResult);

            var result = await negotiator.NegotiateAsync(transceiver, settings);

            Assert.Equal(result.Data[dataName], (dataValue));
        }

        private IFeatureNegotiator CreateAndAddFeatureNegotiator(
            string featureName,
            bool negotiationResult)
        {
            return CreateAndAddFeatureNegotiator(featureName, new FeatureNegotiationResult(negotiationResult));
        }

        private IFeatureNegotiator CreateAndAddFeatureNegotiator(string featureName, FeatureNegotiationResult result)
        {
            var featureNegotiator = A.Fake<IFeatureNegotiator>();
            A.CallTo(() => featureNegotiator.FeatureName).Returns(featureName);
            A.CallTo(() => featureNegotiator.Negotiate(A<ITransceiver<XmlElement, XmlElement>>._, A<XmlElement>._))
                .Returns(result);
            A.CallTo(() => featureNegotiator.NegotiateAsync(A<ITransceiver<XmlElement, XmlElement>>._, A<XmlElement>._))
                .Returns(Task.FromResult(result));

            negotiator = new ClientStreamNegotiator(new[] { featureNegotiator });

            return featureNegotiator;
        }

        private void AddStreamHeaderAndFeaturesResponse(params string[] features)
        {
            AddStreamHeaderResponse();
            AddFeaturesResponse(features);
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
