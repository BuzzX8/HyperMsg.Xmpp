using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IRosterService
    {
        Task<IReadOnlyList<RosterItem>> GetRosterAsync(CancellationToken cancellationToken);

        Task AddOrUpdateItemAsync(RosterItem rosterItem, CancellationToken cancellationToken);

        Task RemoveItemAsync(RosterItem rosterItem, CancellationToken cancellationToken);
    }
}
