using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IRosterService
    {
        Task<string> RequestRosterAsync(Jid entityJid, CancellationToken cancellationToken);

        Task<string> AddOrUpdateItemAsync(Jid entityJid, RosterItem rosterItem, CancellationToken cancellationToken);

        Task<string> RemoveItemAsync(Jid entityJid, RosterItem rosterItem, CancellationToken cancellationToken);

        event Action<RosterResultEventArgs> RosterRequestResult;

        event Action<string> RosterUpdated;
    }
}
