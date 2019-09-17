using HyperMsg.Integration;
using HyperMsg.Transport.Socket;
using HyperMsg.Xmpp.Client.Components;
using HyperMsg.Xmpp.Serialization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class ConnectionTests : IntegrationFixtureBase<XmlElement>
    {
        private readonly ConnectionComponent connectionComponent;
        private readonly XmppConnectionSettings settings;
        private readonly Jid userJid;

        private readonly CancellationTokenSource tokenSource;

        public ConnectionTests()
        {
            userJid = "user@domain.com";
            settings = new XmppConnectionSettings(userJid);
            connectionComponent = new ConnectionComponent(MessageSender, settings);
            tokenSource = new CancellationTokenSource();

            Transport.TransportEvent += connectionComponent.HandleTransportEventAsync;
            HandlerRegistry.Register(connectionComponent.HandleAsync);            
        }

        [Fact]
        public async Task Transport_Open()
        {            
            await Transport.ProcessCommandAsync(TransportCommand.Open, tokenSource.Token);

            await Task.Delay(1000);
        }

        protected override void ConfigureSerializer(IConfigurable configurable)
        {
            configurable.RegisterService(typeof(ISerializer<XmlElement>), (p, s) => new XmppSerializer());
        }

        protected override void ConfigureTransport(IConfigurable configurable)
        {
            var adddresses = Dns.GetHostAddresses("jabber.org");
            configurable.UseSockets(new IPEndPoint(adddresses[0], XmppConnectionSettings.DefaultPort));
        }
    }
}