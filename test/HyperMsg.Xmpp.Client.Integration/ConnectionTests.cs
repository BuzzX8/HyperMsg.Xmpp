using HyperMsg.Xmpp.Client.Components;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class ConnectionTests
    {
        private readonly ConnectionComponent connectionComponent;
        private readonly XmppConnectionSettings settings;
        private readonly Jid userJid;

        public ConnectionTests()
        {
            userJid = "user@domain.com";
            settings = new XmppConnectionSettings(userJid);
            connectionComponent = new ConnectionComponent(null, settings);
        }

        [Fact]
        public void Transport_Open()
        {

        }
    }
}