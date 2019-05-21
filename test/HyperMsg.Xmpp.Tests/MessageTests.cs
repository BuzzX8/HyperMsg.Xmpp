using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Tests
{
    public class MessageTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] { new XmlElement("message"), Message.New() };
            yield return new object[] { new XmlElement("message").Type("chat"), Message.Chat() };
            yield return new object[] { new XmlElement("message").Type("error"), Message.Error() };
            yield return new object[] { new XmlElement("message").Type("groupchat"), Message.GroupChat() };
            yield return new object[] { new XmlElement("message").Type("headline"), Message.Headline() };
            yield return new object[] { new XmlElement("message").Type("normal"), Message.Normal() };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void MessageCreateMethodTest(XmlElement expectedStanza, XmlElement actualStanza)
        {
            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public void Subject_Adds_Subject_Child()
        {
            var expectedSubject = Guid.NewGuid().ToString();
            var stanza = Message.New().Subject(expectedSubject);

            var actualSubject = stanza.Child("subject").Value;

            Assert.Equal(expectedSubject, actualSubject);
        }

        [Fact]
        public void Body_Adds_Body_Child()
        {
            var expectedBody = Guid.NewGuid().ToString();
            var stanza = Message.New().Body(expectedBody);

            var actualBody = stanza.Child("body").Value;

            Assert.Equal(expectedBody, actualBody);
        }
    }
}