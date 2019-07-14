using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class MessageServiceTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly Jid jid;

        private readonly MessageService messageService;

        public MessageServiceTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            jid = $"{Guid.NewGuid()}@domain.com";
            messageService = new MessageService(messageSender, jid);
        }

        [Fact]
        public async Task SendMessageAsync_Sends_Correct_Message_Stanza()
        {
            var to = $"{Guid.NewGuid()}@domain.com";
            var message = new Message
            {
                Type = MessageType.Chat,
                Subject = Guid.NewGuid().ToString(),
                Body = Guid.NewGuid().ToString()
            };
            var cancellationToken = default(CancellationToken);
            var expectedStanza = new XmlElement("message")
                .Type("chat")
                .Subject(message.Subject)
                .Body(message.Body);
            var actualStanza = default(XmlElement);
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, cancellationToken)).Invokes(foc =>
            {
                actualStanza = foc.GetArgument<XmlElement>(0);
            });

            await messageService.SendMessageAsync(to, message, cancellationToken);

            Assert.Equal(expectedStanza, actualStanza);
        }
    }
}