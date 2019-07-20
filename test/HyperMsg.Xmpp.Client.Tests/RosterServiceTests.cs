using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class RosterServiceTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly RosterService rosterService;

        private readonly CancellationToken cancellationToken;
        private readonly List<XmlElement> sentRequests;
        private readonly Jid jid;

        public RosterServiceTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            jid = $"{Guid.NewGuid()}@domain.com";
            rosterService = new RosterService(messageSender, jid);
            cancellationToken = new CancellationToken();
            sentRequests = new List<XmlElement>();

            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                sentRequests.Add(foc.GetArgument<XmlElement>(0));
            });
        }

        [Fact]
        public void GetRosterAsync_Sends_Roster_Request_Stanza()
        {
            var _ = rosterService.GetRosterAsync(cancellationToken);

            var request = sentRequests.SingleOrDefault();

            Assert.NotNull(request);            
            Assert.True(request.IsIq());
            Assert.True(request.IsType(IqStanza.Type.Get));
            Assert.Equal(jid, request["from"]);
            Assert.NotNull(request["id"]);
            var query = request.Child("query");
            Assert.NotNull(query);
            Assert.Equal(XmppNamespaces.Roster, query.Xmlns());
        }

        [Fact]
        public void GetRosterAsync_Completes_Task_When_Result_Received()
        {
            var items = Enumerable.Range(0, 5).Select(i => new RosterItem($"user{i}.domain.com", $"name{i}"));            
            var task = rosterService.GetRosterAsync(cancellationToken);            
            var requestId = sentRequests.Single().Id();
            var result = CreateRosterResult(items).Id(requestId);

            rosterService.Handle(result);

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(items, task.Result);
        }

        [Fact]
        public void AddOrUpdateItemAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var task = rosterService.AddOrUpdateItemAsync(item, cancellationToken);

            var request = sentRequests.SingleOrDefault();

            Assert.NotNull(request);
            Assert.True(request.IsIq());
            Assert.True(request.IsType(IqStanza.Type.Result));
            Assert.Equal(item.Jid, request["from"]);
            Assert.NotNull(request["id"]);
        }

        [Fact]
        public void RemoveItemAsync_Sends_Correct_Request_Stanza()
        {
            var item = new RosterItem("user@domain.com", "user");
            var _ = rosterService.RemoveItemAsync(item, cancellationToken);

            var request = sentRequests.SingleOrDefault();

            Assert.NotNull(request);
            Assert.True(request.IsIq());
            Assert.True(request.IsType(IqStanza.Type.Result));
            Assert.Equal(item.Jid, request["from"]);
            Assert.NotNull(request["id"]);
        }

        private XmlElement CreateRosterResult(IEnumerable<RosterItem> rosterItems)
        {
            var result = IqStanza.Result();
            var items = rosterItems.Select(i => new XmlElement("item").Attribute("jid", i.Jid).Attribute("name", i.Name));
            result.Children.Add(new XmlElement("query", items.ToArray()).Xmlns(XmppNamespaces.Roster));

            return result;
        }
    }
}
