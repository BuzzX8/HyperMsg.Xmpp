using HyperMsg.Xmpp.Client.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class RosterService : IRosterService
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<IReadOnlyList<RosterItem>>> rosterRequets;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> itemRequests;

        private readonly IMessageSender<XmlElement> sender;
        private readonly Jid jid;

        public RosterService(IMessageSender<XmlElement> sender, Jid jid)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.jid = jid ?? throw new ArgumentNullException(nameof(jid));
            rosterRequets = new ConcurrentDictionary<string, TaskCompletionSource<IReadOnlyList<RosterItem>>>();
            itemRequests = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();
        }

        public async Task<IReadOnlyList<RosterItem>> GetRosterAsync(CancellationToken cancellationToken)
        {
            var request = CreateRosterRequest(jid);
            var requestId = await sender.SendWithNewIdAsync(request, cancellationToken);

            var tsc = new TaskCompletionSource<IReadOnlyList<RosterItem>>();
            rosterRequets.TryAdd(requestId, tsc);

            return await tsc.Task;
        }

        private XmlElement CreateRosterRequest(Jid jid) => Roster.Get(jid);

        public async Task AddOrUpdateItemAsync(RosterItem rosterItem, CancellationToken cancellationToken)
        {
            var request = CreateAddOrUpdateItemRequest(rosterItem);
            var requestId = await sender.SendWithNewIdAsync(request, cancellationToken);

            var tsc = new TaskCompletionSource<bool>();
            itemRequests.TryAdd(requestId, tsc);

            await tsc.Task;
        }

        private static XmlElement CreateAddOrUpdateItemRequest(RosterItem rosterItem)
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

            return AttachQuery(Iq.Set().From(rosterItem.Jid), item);
        }

        public async Task RemoveItemAsync(RosterItem rosterItem, CancellationToken cancellationToken)
        {
            var request = CreateRemoveItemRequest(rosterItem.Jid);
            var requestId = await sender.SendWithNewIdAsync(request, cancellationToken);

            var tsc = new TaskCompletionSource<bool>();
            itemRequests.TryAdd(requestId, tsc);

            await tsc.Task;
        }

        private static XmlElement CreateRemoveItemRequest(Jid itemJid)
        {
            var itemElement = new XmlElement("item");
            itemElement.SetAttributeValue("jid", itemJid);
            itemElement.SetAttributeValue("subscription", "remove");

            return AttachQuery(Iq.Set().From(itemJid), itemElement);
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
            if (iqStanza.IsResult() && iqStanza.HasChild("query"))
            {
                if (rosterRequets.TryRemove(iqStanza.Id(), out var tsc))
                {
                    var rosterItems = ToRosterItems(iqStanza.Child("query").Children);
                    tsc.SetResult(rosterItems);
                }
            }
        }

        private IReadOnlyList<RosterItem> ToRosterItems(IEnumerable<XmlElement> items)
        {
            return items.Select(i => new RosterItem(i["jid"], i["name"])).ToArray();
        }
    }
}
