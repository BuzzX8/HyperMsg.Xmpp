using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public class MessageExtensionTests : SenderExtensionTestBase
    {
        private static readonly Jid to = "to-user@domain.org";
        private static readonly string text = Guid.NewGuid().ToString();

        public static IEnumerable<object[]> GetTestCasesForSendMessage()
        {
            yield return new object[]
            {
                MessageType.Chat,
                Message.Chat().To(to).Body(text)
            };

            yield return new object[]
            {
                MessageType.Error,
                Message.Error().To(to).Body(text)
            };

            yield return new object[]
            {
                MessageType.GroupChat,
                Message.GroupChat().To(to).Body(text)
            };

            yield return new object[]
            {
                MessageType.Headline,
                Message.Headline().To(to).Body(text)
            };

            yield return new object[]
            {
                MessageType.Normal,
                Message.Normal().To(to).Body(text)
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForSendMessage))]
        public void SendMessage_Sends_Correct_Message_Stanza(MessageType type, XmlElement expectedStanza)
        {
            VerifySendMethod((s, e) => s.SendMessage(to, type, text), expectedStanza);
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForSendMessage))]
        public async Task SendMessageAsync_Sends_Correct_Message_Stanza(MessageType type, XmlElement expectedStanza)
        {
            await VerifySendAsyncMethod((s, e) => s.SendMessageAsync(to, type, text), expectedStanza);
        }

        [Fact]
        public void SendMessage_Sends_Message_Stanza_With_Correct_Subject()
        {
            string subject = "subj";
            var expectedStanza = new XmlElement("message").Type("chat").To(to)
                .Children(new XmlElement("subject").Value(subject), new XmlElement("body").Value(text));

             VerifySendMethod((s, e) => s.SendMessage(to, MessageType.Chat, subject, text), expectedStanza);
        }

        [Fact]
        public async Task SendMessageAsync_Sends_Message_Stanza_With_Correct_Subject()
        {
            string subject = "subj";
            var expectedStanza = new XmlElement("message").Type("chat").To(to)
                .Children(new XmlElement("subject").Value(subject), new XmlElement("body").Value(text));

            await VerifySendAsyncMethod((s, e) => s.SendMessageAsync(to, MessageType.Chat, subject, text), expectedStanza);
        }
    }
}
