using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class RosterExtensions
    {
        public static string SendRosterRequest(this ISender<XmlElement> channel)
        {
            var stanza = CreateRosterRequest();
            return channel.SendWithNewId(stanza);
        }

        public static Task<string> SendRosterRequestAsync(this ISender<XmlElement> channel)
        {
            var stanza = CreateRosterRequest();
            return channel.SendWithNewIdAsync(stanza);
        }

        public static string SendAddOrUpdateItemRequest(this ISender<XmlElement> channel, Jid jid, string name = null, params string[] groups)
        {
            var stanza = CreateAddOrUpdateItemRequest(jid, name, groups);
            return channel.SendWithNewId(stanza);
        }

        public static Task<string> SendAddOrUpdateItemRequestAsync(this ISender<XmlElement> channel, Jid jid, string name = null, params string[] groups)
        {
            var stanza = CreateAddOrUpdateItemRequest(jid, name, groups);
            return channel.SendWithNewIdAsync(stanza);
        }

        public static string SendRemoveItemRequest(this ISender<XmlElement> channel, Jid itemJid)
        {
            var stanza = CreateRemoveItemRequest(itemJid);
            return channel.SendWithNewId(stanza);
        }

        public static Task<string> SendRemoveItemRequestAsync(this ISender<XmlElement> channel, Jid itemJid)
        {
            var stanza = CreateRemoveItemRequest(itemJid);
            return channel.SendWithNewIdAsync(stanza);
        }

        private static XmlElement CreateRosterRequest()
        {
            return AttachQuery(Iq.Get());
        }

        private static XmlElement CreateAddOrUpdateItemRequest(Jid itemJid, string name, string[] groups)
        {
            var item = new XmlElement("item").Attribute("jid", itemJid);

            if (!string.IsNullOrEmpty(name))
            {
                item.SetAttributeValue("name", name);
            }

            for (int i = 0; i < groups.Length; i++)
            {
                item.Children.Add(new XmlElement("group").Value(groups[i]));
            }

            return AttachQuery(Iq.Set(), item);
        }

        private static XmlElement CreateRemoveItemRequest(Jid itemJid)
        {
            var itemElement = new XmlElement("item");
            itemElement.SetAttributeValue("jid", itemJid);
            itemElement.SetAttributeValue("subscription", "remove");

            return AttachQuery(Iq.Set(), itemElement);
        }

        private static XmlElement AttachQuery(XmlElement element, params XmlElement[] items)
        {
            var query = new XmlElement("query").Xmlns(XmppNamespaces.Roster);
            element.Children.Add(query);
            query.Children(items);

            return element;
        }
    }
}
