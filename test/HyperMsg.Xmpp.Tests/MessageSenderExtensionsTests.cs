using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System;

namespace HyperMsg.Xmpp
{
    public class MessageSenderExtensionsTests : ServiceHostFixture
    {        
        private readonly Jid jid = "user@domain.com";

        public MessageSenderExtensionsTests()
        { }

        #region Roster

        [Fact]
        public async Task SendRosterRequestAsync_Sends_Roster_Request_Stanza()
        {
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Get).From(jid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(s => actualStanza = s);

            var requestId = await MessageSender.SendRosterRequestAsync(jid);
            expectedStanza.Id(requestId);

            Assert.False(string.IsNullOrEmpty(requestId));
            Assert.Equal(expectedStanza, actualStanza);
        }        

        [Fact]
        public async Task AddOrUpdateItemAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Set, item).From(jid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(s => actualStanza = s);

            var requestId = await MessageSender.SendRosterItemUpdateAsync(jid, item);
            expectedStanza.Id(requestId);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task RemoveItemAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Set, item).From(jid);
            var itemElement = expectedStanza.Child("query").Child("item");
            itemElement.SetAttributeValue("subscription", "remove");
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(s => actualStanza = s);
            

            var requestId = await MessageSender.RemoveItemAsync(jid, item);
            expectedStanza.Id(requestId);

            Assert.Equal(expectedStanza, actualStanza);
        }

        private XmlElement CreateRosterStanza(string type, params RosterItem[] rosterItems)
        {
            var result = IqStanza.New().Type(type);
            var items = rosterItems.Select(i => new XmlElement("item").Attribute("jid", i.Jid).Attribute("name", i.Name));
            result.Children.Add(new XmlElement("query", items.ToArray()).Xmlns(XmppNamespaces.Roster));

            return result;
        }

        #endregion

        #region Presence

        [Fact]
        public async Task UpdateStatusAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStatus = new PresenceStatus
            {
                AvailabilitySubstate = AvailabilitySubstate.Chat,
                StatusText = Guid.NewGuid().ToString()
            };
            var token = new CancellationToken();
            var sentStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => sentStanza = e);

            await MessageSender.UpdateStatusAsync(expectedStatus, token);

            Assert.NotNull(sentStanza);
            Assert.Equal(expectedStatus.StatusText, sentStanza.Child("status").Value);
            Assert.Equal(expectedStatus.AvailabilitySubstate.ToString().ToLower(), sentStanza.Child("show").Value);
        }

        #endregion

        #region Subscription

        [Fact]
        public async Task ApproveSubscriptionAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Subscribed).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.ApproveSubscriptionAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task CancelSubscriptionAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribed).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.CancelSubscriptionAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task RequestSubscriptionAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Subscribe).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.RequestSubscriptionAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task UnsubscribeAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribe).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.UnsubscribeAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        #endregion
    }
}
