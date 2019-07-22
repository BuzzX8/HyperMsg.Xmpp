using System;
using System.Collections.Generic;

namespace HyperMsg.Xmpp.Client
{
    public class RosterResultEventArgs : EventArgs
    {
        public RosterResultEventArgs(string responseId, IReadOnlyList<RosterItem> roster)
        {
            ResponseId = responseId ?? throw new ArgumentNullException(nameof(responseId));
            Roster = roster ?? throw new ArgumentNullException(nameof(roster));
        }

        public string ResponseId { get; }

        public IReadOnlyList<RosterItem> Roster { get; }
    }
}
