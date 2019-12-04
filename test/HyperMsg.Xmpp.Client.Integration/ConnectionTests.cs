using HyperMsg.Integration;
using HyperMsg.Socket;
using System.Net;
using Xunit;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class ConnectionTests : IntegrationFixtureBase
    {
        const int DefaultBufferSize = 2048;
              

        private static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, XmppConnectionSettings.DefaultPort);

        public ConnectionTests() : base(DefaultBufferSize, DefaultBufferSize)
        {
            Configurable.UseXmppServices(null);            
            Configurable.UseSockets(endPoint);
        }

        [Fact]
        public async Task Open_Xmpp_Connection_()
        {
            await OpenTransportAsync();
        }
    }
}