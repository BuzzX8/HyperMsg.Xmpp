using FakeItEasy;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Components
{
    public class RosterComponentTests
    {
        private readonly IMessageSender messageSender;
        private readonly RosterComponent rosterService;

        private readonly CancellationToken cancellationToken;
        private readonly Jid entityJid;

        public RosterComponentTests()
        {
            messageSender = A.Fake<IMessageSender>();
            entityJid = $"{Guid.NewGuid()}@domain.com";
            rosterService = new RosterComponent(messageSender);
            cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task RequestRosterAsync_Sends_Roster_Request_Stanza()
        {
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Get).From(entityJid);

            var requestId = await rosterService.RequestRosterAsync(entityJid, cancellationToken);
            expectedStanza.Id(requestId);

            Assert.False(string.IsNullOrEmpty(requestId));
            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }        

        [Fact]
        public async Task AddOrUpdateItemAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Set, item).From(entityJid);

            var requestId =  await rosterService.AddOrUpdateItemAsync(entityJid, item, cancellationToken);
            expectedStanza.Id(requestId);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task RemoveItemAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var expectedStanza = CreateRosterStanza(IqStanza.Type.Set, item).From(entityJid);
            var itemElement = expectedStanza.Child("query").Child("item");            
            itemElement.SetAttributeValue("subscription", "remove");

            var requestId = await rosterService.RemoveItemAsync(entityJid, item, cancellationToken);
            expectedStanza.Id(requestId);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public void Handle_Rises_RosterResult_When_Roster_Result_Received()
        {
            var items = Enumerable.Range(0, 5).Select(i => new RosterItem($"user{i}.domain.com", $"name{i}"));
            var responseId = Guid.NewGuid().ToString();
            var actualArgs = default(RosterResultEventArgs);
            rosterService.RosterRequestResult += e => actualArgs = e;
            var result = CreateRosterStanza(IqStanza.Type.Result, items.ToArray()).Id(responseId);

            rosterService.Handle(result);

            Assert.NotNull(actualArgs);
            Assert.Equal(responseId, actualArgs.ResponseId);
            Assert.Equal(items, actualArgs.Roster);
        }

        private XmlElement CreateRosterStanza(string type, params RosterItem[] rosterItems)
        {
            var result = IqStanza.New().Type(type);
            var items = rosterItems.Select(i => new XmlElement("item").Attribute("jid", i.Jid).Attribute("name", i.Name));
            result.Children.Add(new XmlElement("query", items.ToArray()).Xmlns(XmppNamespaces.Roster));

            return result;
        }
    }
}
