using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class TlsNegotiatorTests
    {
        //private ITransportContext context;
        private TlsNegotiator negotiator;
        private ITransceiver<XmlElement, XmlElement> channel;

        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(1);

        public TlsNegotiatorTests()
        {
            //context = A.Fake<ITransportContext>();
            negotiator = new TlsNegotiator();
            channel = null;
        }

        [Fact]
        public void Negotiate_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            Assert.Throws<XmppException>(() => negotiator.Negotiate(channel, feature));
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, feature));
        }

        [Fact]
        public void Negotiate_Sends_StartTls()
        {
            //channel.IsManualSync = true;
            var task = Task.Run(() => negotiator.Negotiate(channel, Tls.Start()));
            //channel.WaitForSend(waitTimeout);

            var actualElement = default(XmlElement);// channel.SentElements.Single();

            Assert.Equal(actualElement, Tls.Start());
        }

        [Fact]
        public void NegotiateAsync_Sends_StartTls()
        {
            //channel.IsManualSync = true;
            var task = negotiator.NegotiateAsync(channel, Tls.Start());
            //channel.WaitForSend(waitTimeout);

            var actualElement = default(XmlElement);// channel.SentElements.Single();

            Assert.Equal(actualElement, Tls.Start());
        }

        [Fact]
        public void Negotiate_Throws_Exception_If_Invalid_Response_Received()
        {
            //channel.EnqueueResponse(new XmlElement("invalid-element"));

            Assert.Throws<XmppException>(() => negotiator.Negotiate(channel, Tls.Start()));
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Response_Received()
        {
            //channel.EnqueueResponse(new XmlElement("invalid-element"));

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, Tls.Start()));
        }

        [Fact]
        public void Negotiate_Throws_Exception_If_Failure_Received()
        {
            //channel.EnqueueResponse(Tls.Failure());

            Assert.Throws<XmppException>(() => negotiator.Negotiate(channel, Tls.Start()));
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Failure_Received()
        {
            //channel.EnqueueResponse(Tls.Failure());

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(channel, Tls.Start()));
        }

        [Fact]
        public void Negotiate_Throws_Exception_If_Context_Does_Not_Supports_Tls()
        {
            //channel.EnqueueResponse(Tls.Proceed());

            Assert.Throws<NotSupportedException>(() => negotiator.Negotiate(channel, Tls.Start()));
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Context_Does_Not_Supports_Tls()
        {
            //channel.EnqueueResponse(Tls.Proceed());

            await Assert.ThrowsAsync<NotSupportedException>(() => negotiator.NegotiateAsync(channel, Tls.Start()));
        }

        [Fact]
        public void Negotiate_Sets_Tls_Stream()
        {
            //channel.EnqueueResponse(Tls.Proceed());
            //var tlsContext = A.Fake<IClientTlsContext>();
            //A.CallTo(() => context.GetService(typeof(IClientTlsContext))).Returns(tlsContext);

            var result = negotiator.Negotiate(channel, Tls.Start());

            Assert.True(result.IsStreamRestartRequired);
            //A.CallTo(() => tlsContext.SetTlsStream()).MustHaveHappened();
        }

        [Fact]
        public async Task NegotiateAsync_Sets_Tls_Stream()
        {
            //channel.EnqueueResponse(Tls.Proceed());
            //var tlsContext = A.Fake<IClientTlsContext>();
            //A.CallTo(() => context.GetService(typeof(IClientTlsContext))).Returns(tlsContext);

            var result = await negotiator.NegotiateAsync(channel, Tls.Start());

            Assert.True(result.IsStreamRestartRequired);
            //A.CallTo(() => tlsContext.SetTlsStreamAsync(A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
