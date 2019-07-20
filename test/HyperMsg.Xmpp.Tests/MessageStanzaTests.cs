using System;
using Xunit;

namespace HyperMsg.Xmpp.Tests
{
    public class MessageStanzaTests
    {

        [Fact]
        public void New_Creates_Message_Stanza_With_Provided_Type()
        {
            var expectedType = Guid.NewGuid().ToString();
            var stanza = MessageStanza.New(expectedType);

            Assert.Equal("message", stanza.Name);
            Assert.Equal(expectedType, stanza.Type());
        }

        [Fact]
        public void New_Creates_Message_Stanza_With_Provided_Subject()
        {
            var expectedSubject = Guid.NewGuid().ToString();
            var stanza = MessageStanza.New(MessageStanza.Type.Chat, expectedSubject);

            var actualSubject = stanza.Child("subject").Value;

            Assert.Equal(expectedSubject, actualSubject);
        }

        [Fact]
        public void New_Creates_Message_Stanza_With_Provided_Body()
        {
            var expectedBody = Guid.NewGuid().ToString();
            var stanza = MessageStanza.New(MessageStanza.Type.Error, null, expectedBody);

            var actualBody = stanza.Child("body").Value;

            Assert.Equal(expectedBody, actualBody);
        }
    }
}