using System;

namespace HyperMsg.Xmpp.Client
{
    public class RosterItem
    {
        public RosterItem(Jid jid)
        {
            Jid = jid ?? throw new ArgumentNullException(nameof(jid));
        }

        public Jid Jid { get; }

        public string Name { get; set; }
    }
}
