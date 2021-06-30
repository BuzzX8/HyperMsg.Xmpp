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
        public async Task SendRosterItemUpdateAsync_Sends_Correct_Request_Stanza()
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
        public async Task SendRosterItemRemoveRequestAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Set, item).From(jid);
            var itemElement = expectedStanza.Child("query").Child("item");
            itemElement.SetAttributeValue("subscription", "remove");
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(s => actualStanza = s);
            

            var requestId = await MessageSender.SendRosterItemRemoveRequestAsync(jid, item);
            expectedStanza.Id(requestId);

            Assert.Equal(expectedStanza, actualStanza);
        }

        private static XmlElement CreateRosterStanza(string type, params RosterItem[] rosterItems)
        {
            var result = IqStanza.New().Type(type);
            var items = rosterItems.Select(i => new XmlElement("item").Attribute("jid", i.Jid).Attribute("name", i.Name));
            result.Children.Add(new XmlElement("query", items.ToArray()).Xmlns(XmppNamespaces.Roster));

            return result;
        }

        #endregion

        #region Presence

        [Fact]
        public async Task SendStatusUpdateAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStatus = new PresenceStatus
            {
                AvailabilitySubstate = AvailabilitySubstate.Chat,
                StatusText = Guid.NewGuid().ToString()
            };
            var token = new CancellationToken();
            var sentStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => sentStanza = e);

            await MessageSender.SendStatusUpdateAsync(expectedStatus, token);

            Assert.NotNull(sentStanza);
            Assert.Equal(expectedStatus.StatusText, sentStanza.Child("status").Value);
            Assert.Equal(expectedStatus.AvailabilitySubstate.ToString().ToLower(), sentStanza.Child("show").Value);
        }

        #endregion

        #region Subscription

        [Fact]
        public async Task SendSubscriptionApprovalAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Subscribed).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.SendSubscriptionApprovalAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task SendSubscriptionCancellationAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribed).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.SendSubscriptionCancellationAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task SendSubscriptionRequestAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Subscribe).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.SendSubscriptionRequestAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public async Task SendUnsubscribeRequestAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribe).To(subscriptionJid);
            var actualStanza = default(XmlElement);
            HandlersRegistry.RegisterTransmitPipeHandler<XmlElement>(e => actualStanza = e);

            await MessageSender.SendUnsubscribeRequestAsync(subscriptionJid);

            Assert.Equal(expectedStanza, actualStanza);
        }

        #endregion
    }
}
