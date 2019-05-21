using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IXmppClient
    {
        void Connect();

        Task ConnectAsync();

        void Disconnect();

        Task DisconnectAsync();
    }
}
