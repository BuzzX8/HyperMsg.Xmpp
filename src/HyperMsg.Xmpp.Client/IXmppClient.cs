using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IXmppClient
    {
        void Connect();

        Task ConnectAsync(CancellationToken token = default);

        void Disconnect();

        Task DisconnectAsync(CancellationToken token = default);
    }
}
