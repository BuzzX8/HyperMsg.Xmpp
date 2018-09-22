using Xunit;

namespace HyperMsg.Xmpp.Tests
{
    public class RosterTests
    {
        [Fact]
        public void Get_Returns_Element_With_Roster_Request()
        {
            var element = Roster.Get();

            VerifyRosterStanza(element);
            Assert.True(element.IsGet());
        }

        [Fact]
        public void Get_Returns_Roster_Request_With_Specifiet_From_Value()
        {
            var from = "user@domain";

            var element = Roster.Get(from);

            VerifyRosterStanza(element);
            Assert.Equal(element["from"], from);
            Assert.True(element.IsGet());
        }

        [Fact]
        public void Set_Returns_Roster_Update_Element()
        {
            Jid jid = "user@domain";
            string name = "User";

            var element = Roster.Set(jid, name);

            VerifyRosterStanza(element);
            Assert.True(element.IsSet());

            var item = element.Child("query").Child("item");

            Assert.Equal(item["jid"], jid.ToString());
            Assert.Equal(item["name"], name);
        }

        [Fact]
        public void Set_Returns_Roster_Update_Element_With_Groups()
        {
            string[] groups = { "group1", "group2" };

            var element = Roster.Set("user@domain", "User", groups);

            var groupElements = element.Child("query").Child("item").Children;

            Assert.Equal(groupElements.Count, groups.Length);

            foreach (var group in groupElements)
            {
                Assert.Equal("group", group.Name);
                Assert.Contains(group.Value, groups);
            }
        }

        [Fact]
        public void Remove_Returns_Remove_Roster_Item_Element()
        {
            Jid jid = "user@domain";

            var element = Roster.Remove(jid);

            VerifyRosterStanza(element);
            Assert.True(element.IsSet());
            Assert.Equal("remove", element.Child("query").Child("item")["subscription"]);
        }

        private void VerifyRosterStanza(XmlElement element)
        {
            Assert.True(element.IsIq());
            Assert.True(element.HasChild("query"));
            Assert.Equal(element.Child("query").Xmlns(), Roster.RosterXmlns);
        }
    }
}
