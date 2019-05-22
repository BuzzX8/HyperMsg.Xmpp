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
        private ITransceiver<XmlElement, XmlElement> channel;
        private XmppConnectionSettings settings;
        private string domain = "Fact-domain";
        private TimeSpan waitTime = TimeSpan.FromSeconds(1);

        public ClientStreamNegotiatorTests()
        {
            negotiator = new ClientStreamNegotiator(Enumerable.Empty<IFeatureNegotiator>());
            channel = A.Fake<ITransceiver<XmlElement, XmlElement>>();
            settings = new XmppConnectionSettings(domain);
        }

        [Fact]
        public void Negotiate_Throws_Exception_If_Domain_In_Settings_Is_Null()
        {
            //settings.Domain = null;
            EnqueueStreamHeaderAndFeatures("feature");

            Assert.Throws<ArgumentException>(() => negotiator.Negotiate(channel, settings));
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Domain_In_Settings_Is_Null()
        {
            //settings.Domain = null;
            EnqueueStreamHeaderAndFeatures("feature");

            await Assert.ThrowsAsync<ArgumentException>(() => negotiator.NegotiateAsync(channel, settings));
        }

        [Fact]
        public void Negotiate_Sends_Stream_Header()
        {
            //channel.IsManualSync = true;
            var task = Task.Run(() => negotiator.Negotiate(channel, settings));
            //channel.WaitForSend(waitTime);

            var header = default(XmlElement);// channel.SentElements.Single();

            VerifyStreamHeader(header);
        }

        [Fact]
        public void NegotiateAsync_Sends_StreamHeader()
        {
            //channel.IsManualSync = true;
            var task = negotiator.NegotiateAsync(channel, settings);
            //channel.WaitForSend(waitTime);

            var header = default(XmlElement);// channel.SentElements.Single();

            VerifyStreamHeader(header);
        }

        private void VerifyStreamHeader(XmlElement element)
        {
            Assert.Equal(element.Name, "stream:stream");
            Assert.Equal(element["xmlns"], XmppNamespaces.JabberClient);
            Assert.Equal(element["xmlns:stream"], XmppNamespaces.Streams);
            Assert.Equal(element["to"], domain);
        }

        [Fact]
        public async Task Negotiate_Throws_Exception_If_Invalid_Header_Received()
        {
            //channel.IsManualSync = true;
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);
            //channel.EnqueueResponse(incorrectHeader);
            var task = Task.Run(() => negotiator.Negotiate(channel, settings));
            //channel.WaitForSend(waitTime);
            //channel.ReceiveNext();

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Header_Received()
        {
            //channel.IsManualSync = true;
            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);
            //channel.EnqueueResponse(incorrectHeader);
            var task = negotiator.NegotiateAsync(channel, settings);
            //channel.WaitForSend(waitTime);
            //channel.ReceiveNext();

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task Negotiate_Throws_Exception_If_Incorrect_Features_Received()
        {
            EnqueueStreamHeader();
            //channel.EnqueueResponse(new XmlElement("stream:incorrect-features"));
            var task = Task.Run(() => negotiator.Negotiate(channel, settings));
            //channel.WaitForSend(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Incorrect_Features_Received()
        {
            EnqueueStreamHeader();
            //channel.EnqueueResponse(new XmlElement("stream:incorrect-features"));
            var task = negotiator.NegotiateAsync(channel, settings);
            //channel.WaitForSend(waitTime);

            await Assert.ThrowsAsync<XmppException>(() => task);
        }

        [Fact]
        public void Negotiate_Does_Not_Negotiates_Features_If_No_Negotiators()
        {
            EnqueueStreamHeaderAndFeatures();

            var result = negotiator.Negotiate(channel, settings);

            Assert.Empty(result.NegotiatedFeatures);
        }

        [Fact]
        public async Task NegotiateAsync_Does_Not_Negotiates_Features_If_No_Negotiators()
        {
            EnqueueStreamHeaderAndFeatures();

            var result = await negotiator.NegotiateAsync(channel, settings);

            Assert.Empty(result.NegotiatedFeatures);
        }

        [Fact]
        public void Negotiate_Does_Not_Uses_Negotiator_For_Not_Applicable_Feature()
        {
            EnqueueStreamHeaderAndFeatures("Fact-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator("feature-0", false);

            var result = negotiator.Negotiate(channel, settings);

            Assert.Empty(result.NegotiatedFeatures);
            A.CallTo(() => featureNegotiator.Negotiate(A<ITransceiver<XmlElement, XmlElement>>._, A<XmlElement>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task NegotiateAsync_Does_Not_Uses_Negotiator_For_Not_Applicable_Feature()
        {
            EnqueueStreamHeaderAndFeatures("Fact-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator("feature-0", false);

            var result = await negotiator.NegotiateAsync(channel, settings);

            Assert.Empty(result.NegotiatedFeatures);
            A.CallTo(() => featureNegotiator.Negotiate(A<ITransceiver<XmlElement, XmlElement>>._, A<XmlElement>._))
                .MustNotHaveHappened();
        }

        //[Fact]
        public void Negotiate_Uses_Applicable_Feature_Negotiator_Without_Stream_Restart()
        {
            string featureName = "Fact-feature";
            EnqueueStreamHeaderAndFeatures(featureName);
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, false);

            var result = negotiator.Negotiate(channel, settings);

            //Assert.Equal(channel.SentElements.Count, (1));
            //Assert.Contains(featureName, result.NegotiatedFeature);
        }

        //[Fact]
        public async Task NegotiateAsync_Uses_Applicable_Feature_Negotiator_Without_Stream_Restart()
        {
            string featureName = "Fact-feature";
            EnqueueStreamHeaderAndFeatures(featureName);
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, false);

            var result = await negotiator.NegotiateAsync(channel, settings);

            //Assert.Equal(channel.SentElements.Count, (1));
            //Assert.Contains(result.NegotiatedFeatures, featureName);
        }

        //[Fact]
        public void Negotiate_Uses_Applicable_Feature_Negotiator_With_Stream_Restart()
        {
            string featureName = "Fact-feature";
            EnqueueStreamHeaderAndFeatures(featureName);
            EnqueueStreamHeaderAndFeatures("some-other-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, true);

            var result = negotiator.Negotiate(channel, settings);

            //Assert.Equal(channel.SentElements.Count, (2));
            //Assert.Contains(result.NegotiatedFeatures, featureName);
        }

        //[Fact]
        public async Task NegotiateAsync_Uses_Applicable_Feature_Negotiator_With_Stream_Restart()
        {
            string featureName = "Fact-feature";
            EnqueueStreamHeaderAndFeatures(featureName);
            EnqueueStreamHeaderAndFeatures("some-other-feature");
            var featureNegotiator = CreateAndAddFeatureNegotiator(featureName, true);

            var result = await negotiator.NegotiateAsync(channel, settings);

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
            EnqueueStreamHeaderAndFeatures(featureName);
            CreateAndAddFeatureNegotiator(featureName, featureResult);

            var result = negotiator.Negotiate(channel, settings);

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
            EnqueueStreamHeaderAndFeatures(featureName);
            CreateAndAddFeatureNegotiator(featureName, featureResult);

            var result = await negotiator.NegotiateAsync(channel, settings);

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

        private void EnqueueStreamHeaderAndFeatures(params string[] features)
        {
            EnqueueStreamHeader();
            EnqueueFeatures(features);
        }

        private void EnqueueStreamHeader()
        {
            var header = StreamHeader.Server().From(domain);
            //channel.EnqueueResponse(header);
        }

        private void EnqueueFeatures(params string[] features)
        {
            var element = new XmlElement("stream:features");

            foreach (var feature in features)
            {
                element.Children(new XmlElement(feature));
            }

            //channel.EnqueueResponse(element);
        }
    }
}
