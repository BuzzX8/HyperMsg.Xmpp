using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Components
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
            messageService = new MessageService(messageSender);
        }

        [Fact]
        public async Task SendMessageAsync_Sends_Correct_Message_Stanza()
        {
            var message = CreateMessage();
            var cancellationToken = default(CancellationToken);            
            var actualStanza = default(XmlElement);
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, cancellationToken)).Invokes(foc =>
            {
                actualStanza = foc.GetArgument<XmlElement>(0);
            });

            var messageId = await messageService.SendMessageAsync(jid, message, cancellationToken);

            Assert.NotNull(actualStanza);
            Assert.NotNull(messageId);
            Assert.Equal(messageId, actualStanza.Id());
            var expectedStanza = CreateMessageStanza(messageId, message);
            Assert.Equal(expectedStanza, actualStanza);
        }

        [Fact]
        public void Handle_Rises_MessageReceived_For_Message_Stanza()
        {
            var messageId = Guid.NewGuid().ToString();
            var message = CreateMessage();
            var actualEventArgs = default(MessageReceivedEventArgs);
            messageService.MessageReceived += m => actualEventArgs = m;
            var stanza = CreateMessageStanza(messageId, message).From(jid);

            messageService.Handle(stanza);

            Assert.Equal(actualEventArgs.Id, messageId);
            Assert.Equal(actualEventArgs.Message, message);
            Assert.Equal(actualEventArgs.SenderJid, jid);
        }

        private Message CreateMessage(MessageType messageType = MessageType.Chat)
        {
            return new Message
            {
                Type = messageType,
                Subject = Guid.NewGuid().ToString(),
                Body = Guid.NewGuid().ToString()
            };
        }

        private XmlElement CreateMessageStanza(string id, Message message)
        {
            return MessageStanza.New("chat", message.Subject, message.Body).Id(id).To(jid);
        }
    }
}