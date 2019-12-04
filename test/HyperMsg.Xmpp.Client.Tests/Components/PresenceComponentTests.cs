using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Components
{
    public class PresenceComponentTests
    {
        private readonly IMessageSender messageSender;
        private readonly PresenceComponent presenceService;

        public PresenceComponentTests()
        {
            messageSender = A.Fake<IMessageSender>();
            presenceService = new PresenceComponent(messageSender);
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
            var entityJid = new Jid("user@domain.com");
            var eventArgs = default(PresenceUpdatedEventArgs);
            presenceService.StatusUpdated += e => eventArgs = e;
            var stanza = PresenceStanza.New("", PresenceStanza.ShowStatus.Away, Guid.NewGuid().ToString()).From(entityJid);

            presenceService.Handle(stanza);

            Assert.NotNull(eventArgs);
            Assert.Equal(entityJid, eventArgs.EntityJid);
            Assert.Equal(eventArgs.Status.StatusText, stanza.Child("status").Value);
            Assert.Equal(eventArgs.Status.AvailabilitySubstate.ToString().ToLower(), stanza.Child("show").Value);
        }
    }
}
