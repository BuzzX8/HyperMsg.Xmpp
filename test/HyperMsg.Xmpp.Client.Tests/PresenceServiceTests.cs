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
        }
    }
}
