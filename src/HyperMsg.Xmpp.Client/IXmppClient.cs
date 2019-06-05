using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IXmppClient
    {
        Task ConnectAsync(CancellationToken cancellationToken);

        Task DisconnectAsync(CancellationToken cancellationToken);

        Task<IEnumerable<RosterItem>> GetRosterAsync(CancellationToken cancellationToken);

        Task AddOrUpdateRosterItem(CancellationToken cancellationToken);

        Task DeleteRosterItem(CancellationToken cancellationToken);
    }
}
