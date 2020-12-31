using HyperMsg.Extensions;
using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Extensions
{
    public static class MessagingContextExtensions
    {
        public static Task<MessagingTask<bool>> OpenStreamAsync(this IMessagingContext messagingContext, XmppConnectionSettings connectionSettings, CancellationToken cancellationToken = default)
        {
            return new ConnectTask(messagingContext, cancellationToken).StartAsync(connectionSettings);
        }

        public static async Task<string> SendMessageAsync(this IMessagingContext messagingContext, Jid recipientJid, Message message, CancellationToken cancellationToken = default)
        {
            var messageStanza = CreateMessageStanza(recipientJid, message);
            await messagingContext.Sender.TransmitAsync(messageStanza, cancellationToken);
            return messageStanza.Id();
        }

        private static XmlElement CreateMessageStanza(Jid recipientJid, Message message)
        {
            var type = message.Type.ToString().ToLower();

            return MessageStanza.New(type, message.Subject, message.Body)
                .NewId()
                .To(recipientJid);
        }

        public static async Task<string> RequestRosterAsync(this IMessagingContext messagingContext, Jid entityJid, CancellationToken cancellationToken = default)
        {
            var stanza = CreateRosterRequest(entityJid);
            await messagingContext.Sender.TransmitAsync(stanza, cancellationToken);

            return stanza.Id();
        }

        private static XmlElement CreateRosterRequest(Jid entityJid) => AttachQuery(IqStanza.Get().From(entityJid)).NewId();

        private static XmlElement AttachQuery(XmlElement element, params XmlElement[] items)
        {
            var query = new XmlElement("query").Xmlns(XmppNamespaces.Roster);
            element.Children.Add(query);
            query.Children(items);

            return element;
        }

        public static async Task<string> AddOrUpdateItemAsync(this IMessagingContext messagingContext, Jid entityJid, RosterItem rosterItem, CancellationToken cancellationToken = default)
        {
            var request = CreateAddOrUpdateItemRequest(entityJid, rosterItem);
            await messagingContext.Sender.TransmitAsync(request, cancellationToken);

            return request.Id();
        }

        private static XmlElement CreateAddOrUpdateItemRequest(Jid entityJid, RosterItem rosterItem)
        {
            var item = ToXmlElement(rosterItem);

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

        public static async Task<string> RemoveItemAsync(this IMessagingContext messagingContext, Jid entityJid, RosterItem rosterItem, CancellationToken cancellationToken = default)
        {
            var request = CreateRemoveItemRequest(entityJid, rosterItem);
            await messagingContext.Sender.TransmitAsync(request, cancellationToken);

            return request.Id();
        }

        private static  XmlElement CreateRemoveItemRequest(Jid entityJid, RosterItem item)
        {
            var itemElement = ToXmlElement(item);
            itemElement.SetAttributeValue("subscription", "remove");

            return AttachQuery(IqStanza.Set().From(entityJid), itemElement);
        }

        private static XmlElement ToXmlElement(RosterItem item)
        {
            var element = new XmlElement("item");
            element.SetAttributeValue("jid", item.Jid);

            if (!string.IsNullOrEmpty(item.Name))
            {
                element.SetAttributeValue("name", item.Name);
            }

            return element;
        }
    }
}
