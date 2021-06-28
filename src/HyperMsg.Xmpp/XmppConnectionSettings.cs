using System;
using System.Collections.Generic;

namespace HyperMsg.Xmpp
{
    public class XmppConnectionSettings
    {
        public static readonly int DefaultPort = 5222;

        public XmppConnectionSettings(Jid jid)
        {
            Jid = jid ?? throw new ArgumentNullException(nameof(jid));
        }

        public ICollection<MessagingService> FeatureNegotiators { get; } = new List<MessagingService>();

        public Jid Jid { get; }

        public string Username => Jid.User;

        public string Domain => Jid.Domain;

        public string Resource => Jid.Resource;

        public bool UseTls { get; set; }

        public bool UseSasl { get; set; }
    }
}
