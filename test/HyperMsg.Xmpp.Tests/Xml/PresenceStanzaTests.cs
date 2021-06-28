using System;
using Xunit;

namespace HyperMsg.Xmpp.Xml
{
    public class PresenceStanzaTests
    {
        [Fact]
        public void New_Creates_Presence_Stanza_With_Provided_Type()
        {
            var expectedType = Guid.NewGuid().ToString();
            var stanza = PresenceStanza.New(expectedType);

            Assert.Equal("presence", stanza.Name);
            Assert.Equal(expectedType, stanza["type"]);
        }

        [Fact]
        public void New_Creates_Presence_Stanza_With_Show_Element()
        {
            var show = PresenceStanza.ShowStatus.DoNotDisturb;

            var stanza = PresenceStanza.New("", show);

            Assert.Equal(show, stanza.Child("show").Value);
        }

        [Fact]
        public void New_Creates_Presence_Stanza_With_Status_Element()
        {
            var status = Guid.NewGuid().ToString();

            var stanza = PresenceStanza.New("", null, status);

            Assert.Equal(status, stanza.Child("status").Value);
        }
    }
}