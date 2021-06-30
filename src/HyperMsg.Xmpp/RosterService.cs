using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    internal class RosterService : MessagingService
    {
        public RosterService(IMessagingContext messagingContext) : base(messagingContext)
        { }

        protected override IEnumerable<IDisposable> GetChildDisposables()
        {
            yield return this.RegisterReceivePipeHandler<XmlElement>(HandleStanzaResponse);
        }

        private Task HandleStanzaResponse(XmlElement xmlElement, CancellationToken cancellationToken)
        {
            if (!xmlElement.IsIqStanza())
            {
                return Task.CompletedTask;
            }

            return HandleIqStanza(xmlElement, cancellationToken);
        }

        private Task HandleIqStanza(XmlElement iqStanza, CancellationToken cancellationToken)
        {
            if (IsRosterResultStanza(iqStanza))
            {
                var queryElement = iqStanza.Child("query");
                var items = ToRosterItems(queryElement.Children);
                return this.SendToReceivePipeAsync(items, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private bool IsRosterResultStanza(XmlElement stanza) => stanza.IsType(IqStanza.Type.Result) && stanza.HasChild("query");

        private IReadOnlyList<RosterItem> ToRosterItems(IEnumerable<XmlElement> items) => items.Select(i => new RosterItem(i["jid"], i["name"])).ToArray();
    }
}
