using HyperMsg.Integration;
using HyperMsg.Socket;
using System.Net;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            Configurable.UseSockets(endPoint);
            receivedElements = new List<XmlElement>();
            HandlerRegistry.Register<Received<XmlElement>>(r =>
            {
                receivedElements.Add(r);
            });
        }

        [Fact]
        public async Task Open_Xmpp_Connection_()
        {
            await OpenTransportAsync();

            Assert.NotEmpty(receivedElements);
        }
    }
}