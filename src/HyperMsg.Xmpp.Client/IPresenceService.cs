using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public interface IPresenceService
    {
        Task UpdateStatusAsync(PresenceStatus presenceStatus, CancellationToken cancellationToken);

        event Action<PresenceStatus> StatusUpdateReceived;
    }
}
