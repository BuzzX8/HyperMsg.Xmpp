using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IXmppClient
    {
        Task ConnectAsync(CancellationToken cancellationToken);

        Task DisconnectAsync(CancellationToken cancellationToken);
    }
}
