using System;

namespace HyperMsg.Xmpp.Client
{
    public class RosterItem : IEquatable<RosterItem>
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

        public override int GetHashCode() => Jid.ToString().GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is RosterItem item))
            {
                return false;
            }

            return Equals(item);
        }

        public bool Equals(RosterItem other) => Jid.Equals(other.Jid);
    }
}
