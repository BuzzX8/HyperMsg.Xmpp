using System;

namespace HyperMsg.Xmpp.Client
{
    public class PresenceUpdatedEventArgs : EventArgs
    {
        public PresenceUpdatedEventArgs(Jid entityJid, PresenceStatus status)
        {
            EntityJid = entityJid ?? throw new ArgumentNullException(nameof(entityJid));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public Jid EntityJid { get; }

        public PresenceStatus Status { get; }
    }
}
