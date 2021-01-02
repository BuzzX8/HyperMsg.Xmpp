using HyperMsg.Extensions;
using HyperMsg.Transport;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.FeatureNegotiators
{
    public class TlsNegotiatorTests
    {
        private readonly XmlElement startTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);
        private readonly TlsNegotiator negotiator = new TlsNegotiator();
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        private readonly Host host;
        private readonly IMessagingContext messagingContext;

        public TlsNegotiatorTests()
        {
            var services = new ServiceCollection();
            services.AddMessagingServices();
            host = new Host(services);
            messagingContext = host.Services.GetRequiredService<IMessagingContext>();
        }

        [Fact]
        public async Task NegotiateAsync_Throws_Exception_If_Invalid_Feature_Provided()
        {
            var feature = new XmlElement("invalid-feature");

            await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(messagingContext, feature, tokenSource.Token));
        }

        [Fact]
        public async Task NegotiateAsync_Sends_StartTls()
        {
            var actual = default(XmlElement);
            messagingContext.Observable.OnTransmit<XmlElement>(e => actual = e);
            await negotiator.NegotiateAsync(messagingContext, startTls, tokenSource.Token);

            Assert.Equal(startTls, actual);
        }

        [Fact]
        public async Task Throws_Exception_If_Invalid_Response_Received()
        {
            await negotiator.NegotiateAsync(messagingContext, startTls, tokenSource.Token);
            var response = new XmlElement("invalid-element");

            Assert.Throws<XmppException>(() => messagingContext.Sender.Received(response));
        }

        [Fact]
        public async Task Throws_Exception_If_Tls_Failure_Received()
        {
            var tlsFailure = new XmlElement("failure").Xmlns(XmppNamespaces.Tls);
            await negotiator.NegotiateAsync(messagingContext, startTls, tokenSource.Token);

            Assert.Throws<XmppException>(() => messagingContext.Sender.Received(tlsFailure));
        }

        [Fact]
        public async Task Sets_Tls_Stream()
        {
            var transportCommand = default(TransportCommand?);
            messagingContext.Observable.Subscribe<TransportCommand>(c => transportCommand = c);
            var tlsProceed = new XmlElement("proceed").Xmlns(XmppNamespaces.Tls);
            await negotiator.NegotiateAsync(messagingContext, startTls, tokenSource.Token);

            messagingContext.Sender.Received(tlsProceed);

            Assert.NotNull(transportCommand);
            Assert.Equal(TransportCommand.SetTransportLevelSecurity, transportCommand);
        }
    }
}
