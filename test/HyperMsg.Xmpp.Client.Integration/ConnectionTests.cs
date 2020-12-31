using HyperMsg.Integration;
using HyperMsg.Socket;
using System.Net;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

namespace HyperMsg.Xmpp.Client
{
    public class ConnectionTests : IntegrationFixtureBase
    {
        const int DefaultBufferSize = 2048;
        private static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, XmppConnectionSettings.DefaultPort);

        private readonly XmppConnectionSettings connectionSettings;
        private readonly List<XmlElement> receivedElements;

        public ConnectionTests() : base(DefaultBufferSize, DefaultBufferSize)
        {
            connectionSettings = new XmppConnectionSettings("user@domain.org");
            Configurable.UseXmppServices(connectionSettings);
            Configurable.UseSockets(endPoint, true);
            receivedElements = new List<XmlElement>();
            HandlerRegistry.Register<Received<XmlElement>>(r =>
            {
                receivedElements.Add(r);
            });
        }

        [Fact]
        public async Task Open_Transport__Opens_Xmpp_Stream()
        {
            var @event = new ManualResetEventSlim();
            HandlerRegistry.Register<Received<XmlElement>>(r =>
            {
                @event.Set();
            });

            await OpenTransportAsync();
            @event.Wait(TimeSpan.FromSeconds(5));

            Assert.NotEmpty(receivedElements);
        }
    }
}