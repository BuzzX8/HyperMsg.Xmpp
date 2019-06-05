using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    internal class IqStanzaHandler
    {
        private readonly ISender<XmlElement> sender;

        internal IqStanzaHandler(ISender<XmlElement> sender)
        {
            this.sender = sender;
        }

        internal async Task SendRosterRequestAsync(Jid from, CancellationToken cancellationToken)
        {
            var request = Roster.Get(from);

            await sender.SendAsync(request, cancellationToken);
        }

        internal void Handle(XmlElement iqStanza)
        {

        }
    }
}
