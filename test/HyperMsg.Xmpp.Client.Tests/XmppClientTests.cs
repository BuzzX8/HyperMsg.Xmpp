using FakeItEasy;
using HyperMsg.Xmpp.Client.StreamNegotiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class XmppClientTests
    {
        private readonly XmppClient client;
        private readonly XmlTransceiverFake transceiver;
        private readonly XmppConnectionSettings settings;
        private readonly IPublisher publisher;

        private readonly CancellationToken cancellationToken;

        public XmppClientTests()
        {
            transceiver = new XmlTransceiverFake();
            settings = new XmppConnectionSettings("user@domain");
            publisher = A.Fake<IPublisher>();
            client = new XmppClient(transceiver, settings, publisher);

            cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task ConnectAsync_Submits_OpenConnection_Command()
        {
            await client.ConnectAsync(cancellationToken);

            A.CallTo(() => publisher.PublishAsync(TransportMessage.Open, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task ConnectAsync_Switches_To_Reactive_Mode()
        {
            await client.ConnectAsync(cancellationToken);

            A.CallTo(() => publisher.PublishAsync(ReceiveMode.SetReactive, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task DisconnectAsync_Sends_EndOfStream_Element()
        {
            await client.DisconnectAsync(cancellationToken);

            var request = transceiver.Requests.Single();

            Assert.Equal("/stream:stream", request.Name);
        }

        [Fact]
        public async Task DisconnectAsync_Submits_CloseConnection_Command()
        {
            await client.DisconnectAsync(cancellationToken);

            A.CallTo(() => publisher.PublishAsync(TransportMessage.Close, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public void GetRosterAsync_Sends_Correct_Iq_Stanza()
        {
            var task = client.GetRosterAsync(cancellationToken);
            transceiver.WaitSendCompleted(TimeSpan.FromSeconds(2));

            var request = transceiver.Requests.Single();

            Assert.NotNull(request);            
            Assert.Equal("iq", request.Name);
            Assert.Equal("get", request["type"]);
            Assert.Equal(settings.Jid, request["from"]);
            Assert.NotNull(request["id"]);
            var query = request.Child("query");
            Assert.NotNull(query);
            Assert.Equal(XmppNamespaces.Roster, query.Xmlns());
        }

        [Fact]
        public void GetRosterAsync_Completes_Task_When_Result_Received()
        {
            var items = Enumerable.Range(0, 5).Select(i => new RosterItem($"user{i}.domain.com", $"name{i}"));            
            var task = client.GetRosterAsync(cancellationToken);
            transceiver.WaitSendCompleted(TimeSpan.FromSeconds(2));
            var requestId = transceiver.Requests.Single().Id();
            var result = CreateRosterResult(items).Id(requestId);

            client.Handle(result);

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(items, task.Result);
        }

        private XmlElement CreateRosterResult(IEnumerable<RosterItem> rosterItems)
        {
            var result = Iq.Result();
            var items = rosterItems.Select(i => new XmlElement("item").Attribute("jid", i.Jid).Attribute("name", i.Name));
            result.Children.Add(new XmlElement("query", items.ToArray()).Xmlns(XmppNamespaces.Roster));

            return result;
        }
    }
}
