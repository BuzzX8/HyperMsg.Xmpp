using System;

namespace HyperMsg.Xmpp.Client
{
    public class RosterItem
    {
        public RosterItem(Jid jid)
        {
            Jid = jid ?? throw new ArgumentNullException(nameof(jid));
        }

        public RosterItem(Jid jid, string name) : this(jid)
        {
            Name = name;
        }

        public Jid Jid { get; }

        public string Name { get; set; }
    }
}
