using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class RosterService : IRosterService
    {
        private readonly IMessageSender<XmlElement> messageSender;

        public RosterService(IMessageSender<XmlElement> messageSender)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public async Task<string> RequestRosterAsync(Jid entityJid, CancellationToken cancellationToken)
        {
            var stanza = CreateRosterRequest(entityJid);
            await messageSender.SendAsync(stanza, cancellationToken);

            return stanza.Id();
        }

        private XmlElement CreateRosterRequest(Jid entityJid) => AttachQuery(IqStanza.Get().From(entityJid)).NewId();

        public async Task<string> AddOrUpdateItemAsync(Jid entityJid, RosterItem rosterItem, CancellationToken cancellationToken)
        {
            var request = CreateAddOrUpdateItemRequest(entityJid, rosterItem);
            await messageSender.SendAsync(request, cancellationToken);

            return request.Id();
        }

        private static XmlElement CreateAddOrUpdateItemRequest(Jid entityJid, RosterItem rosterItem)
        {
            var item = new XmlElement("item").Attribute("jid", rosterItem.Jid);

            if (!string.IsNullOrEmpty(rosterItem.Name))
            {
                item.SetAttributeValue("name", rosterItem.Name);
            }

            //for (int i = 0; i < groups.Length; i++)
            //{
            //    item.Children.Add(new XmlElement("group").Value(groups[i]));
            //}

            return AttachQuery(IqStanza.Set().From(entityJid), item);
        }

        public async Task<string> RemoveItemAsync(Jid entityJid, RosterItem rosterItem, CancellationToken cancellationToken)
        {
            var request = CreateRemoveItemRequest(entityJid, rosterItem.Jid);
            await messageSender.SendAsync(request, cancellationToken);

            return request.Id();
        }

        private static XmlElement CreateRemoveItemRequest(Jid entityJid, Jid itemJid)
        {
            var itemElement = new XmlElement("item");
            itemElement.SetAttributeValue("jid", itemJid);
            itemElement.SetAttributeValue("subscription", "remove");

            return AttachQuery(IqStanza.Set().From(entityJid), itemElement);
        }

        private static XmlElement AttachQuery(XmlElement element, params XmlElement[] items)
        {
            var query = new XmlElement("query").Xmlns(XmppNamespaces.Roster);
            element.Children.Add(query);
            query.Children(items);

            return element;
        }

        public void Handle(XmlElement iqStanza)
        {
            if (!IsRosterStanza(iqStanza))
            {
                return;
            }
        }

        private bool IsRosterStanza(XmlElement stanza)
        {
            return stanza.IsIqStanza();
        }

        private IReadOnlyList<RosterItem> ToRosterItems(IEnumerable<XmlElement> items)
        {
            return items.Select(i => new RosterItem(i["jid"], i["name"])).ToArray();
        }

        public event Action<RosterResultEventArgs> RosterRequestResult;

        public event Action<string> RosterUpdated;
    }
}
