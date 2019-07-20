using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class PresenceServiceTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly Jid jid;
        private readonly PresenceService presenceService;

        public PresenceServiceTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            jid = $"{Guid.NewGuid()}@domain.com";
            presenceService = new PresenceService(messageSender, jid);
        }

        [Fact]
        public async Task UpdateStatusAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStatus = new PresenceStatus
            {
                AvailabilitySubstate = AvailabilitySubstate.Chat,
                StatusText = Guid.NewGuid().ToString()
            };
            var token = new CancellationToken();
            var sentStanza = default(XmlElement);
            A.CallTo(() => messageSender.SendAsync(A<XmlElement>._, token)).Invokes(foc => sentStanza = foc.GetArgument<XmlElement>(0));

            await presenceService.UpdateStatusAsync(expectedStatus, token);

            Assert.NotNull(sentStanza);
            Assert.Equal(expectedStatus.StatusText, sentStanza.Child("status").Value);
            Assert.Equal(expectedStatus.AvailabilitySubstate.ToString().ToLower(), sentStanza.Child("show").Value);
        }

        [Fact]
        public void Handle_Rises_StatusUpdateReceived()
        {
            var actualStatus = default(PresenceStatus);
            presenceService.StatusUpdateReceived += s => actualStatus = s;
            var stanza = new XmlElement("presence");
                //.From(jid)
                //.Show("chat")
                //.Status("status-text");

            presenceService.Handle(stanza);

            Assert.NotNull(actualStatus);
            Assert.Equal(actualStatus.StatusText, stanza.Child("status").Value);
            Assert.Equal(actualStatus.AvailabilitySubstate.ToString().ToLower(), stanza.Child("show").Value);
        }
    }
}
