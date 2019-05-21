using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public class RosterExtensionTests : SenderExtensionTestBase
    {
        private readonly Jid itemJid = "some-user@domain";
        private readonly string itemName = "SomeUser";
        private readonly string[] itemGroups = { "Group1", "Group2" };

        [Fact]
        public void SendRosterRequest_Sends_Correct_Iq_Stanza()
        {
            var expectedStanza = Iq.Get().Children(Query());

            VerifySendMethod((c, e) => c.SendRosterRequest(), expectedStanza);
        }

        [Fact]
        public async Task SendRosterRequestAsync_Sends_Correct_Iq_Stanza()
        {
            var expectedStanza = Iq.Get().Children(Query());

            await VerifySendAsyncMethod((c, e) => c.SendRosterRequestAsync(), expectedStanza);
        }

        [Fact]
        public void SendAddOrUpdateItemRequest_Sends_Item_With_Correct_Jid()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid)));

            VerifySendMethod((c, e) => c.SendAddOrUpdateItemRequest(itemJid), expectedStanza);
        }

        [Fact]
        public async Task SendAddOrUpdateItemRequestAsync_Sends_Item_With_Correct_Jid()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid)));

            await VerifySendAsyncMethod((s, e) => s.SendAddOrUpdateItemRequestAsync(itemJid), expectedStanza);
        }

        [Fact]
        public void SendAddOrUpdateItemRequest_Sends_Item_With_Correct_Jid_And_Name()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid, itemName)));

            VerifySendMethod((s, e) => s.SendAddOrUpdateItemRequest(itemJid, itemName), expectedStanza);
        }

        [Fact]
        public async Task SendAddOrUpdateItemRequestAsync_Sends_Item_With_Correct_Jid_And_Name()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid, itemName)));

            await VerifySendAsyncMethod((s, e) => s.SendAddOrUpdateItemRequestAsync(itemJid, itemName), expectedStanza);
        }

        [Fact]
        public void SendAddOrUpdateItemRequest_Sends_Item_With_Correct_Jid_Name_And_Groups()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid, itemName, itemGroups)));

            VerifySendMethod((s, e) => s.SendAddOrUpdateItemRequest(itemJid, itemName, itemGroups), expectedStanza);
        }

        [Fact]
        public async Task SendAddOrUpdateItemRequestAsync_Sends_Item_With_Correct_Jid_Name_And_Groups()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid, itemName, itemGroups)));

            await VerifySendAsyncMethod((s, e) => s.SendAddOrUpdateItemRequestAsync(itemJid, itemName, itemGroups), expectedStanza);
        }

        [Fact]
        public void SendRemoveItemRequest_Sends_Correct_Iq_Stanza()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid).Attribute("subscription", "remove")));

            VerifySendMethod((s, e) => s.SendRemoveItemRequest(itemJid), expectedStanza);
        }

        [Fact]
        public async Task SendRemoveItemRequestAsync_Sends_Correct_Iq_Stanza()
        {
            var expectedStanza = Iq.Set().Children(
                Query(Item(itemJid).Attribute("subscription", "remove")));

            await VerifySendAsyncMethod((s, e) => s.SendRemoveItemRequestAsync(itemJid), expectedStanza);
        }

        private XmlElement Query(params XmlElement[] children) => new XmlElement("query", children).Xmlns(XmppNamespaces.Roster);

        private XmlElement Item(string jid, string name = null, params string[] groups)
        {
            return new XmlElement("item")
                .Attribute("jid", jid)
                .Attribute("name", name)
                .Children(groups
                    .Select(g => new XmlElement("group").Value(g))
                    .ToArray());
        }
    }
}