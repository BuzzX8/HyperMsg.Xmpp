using HyperMsg.Xmpp.Client.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    internal class IqStanzaHandler
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<IEnumerable<RosterItem>>> rosterRequets;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> itemRequests;
        private readonly ISender<XmlElement> sender;

        internal IqStanzaHandler(ISender<XmlElement> sender)
        {
            this.sender = sender;
            rosterRequets = new ConcurrentDictionary<string, TaskCompletionSource<IEnumerable<RosterItem>>>();
            itemRequests = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();
        }

        internal async Task<IEnumerable<RosterItem>> SendRosterRequestAsync(Jid from, CancellationToken cancellationToken)
        {
            var request = CreateRosterRequest(from);
            var requestId = await sender.SendWithNewIdAsync(request, cancellationToken);

            var tsc = new TaskCompletionSource<IEnumerable<RosterItem>>();
            rosterRequets.TryAdd(requestId, tsc);

            return await tsc.Task;
        }

        private XmlElement CreateRosterRequest(Jid jid) => Roster.Get(jid);

        internal async Task SendAddOrUpdateRosterItemAsync(RosterItem rosterItem, CancellationToken cancellationToken)
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

        internal async Task SendItemRemoveRequestAsync(RosterItem rosterItem, CancellationToken cancellationToken)
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

        internal void Handle(XmlElement iqStanza)
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

        private IEnumerable<RosterItem> ToRosterItems(IEnumerable<XmlElement> items)
        {
            return items.Select(i => new RosterItem(i["jid"], i["name"]));
        }
    }
}
