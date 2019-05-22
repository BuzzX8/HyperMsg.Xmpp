using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public class PresenceExtensionTests : SenderExtensionTestBase
    {
        [Fact]
        public void SendPresenceStatus_Sends_Correct_Stanza_For_Available_State()
        {
            var expectedStanza = Presence.New();

            VerifySendMethod((s, e) => s.SendPresenceStatus(true), expectedStanza);
        }

        [Fact]
        public void SendPresenceStatus_Sends_Correct_Stanza_For_Unavailable_State()
        {
            var expectedStanza = Presence.New().Type("unavailable");

            VerifySendMethod((s, e) => s.SendPresenceStatus(false), expectedStanza);
        }

        [Fact]
        public async Task SendPresenceStatusAsync_Sends_Correct_Stanza_For_Available_State()
        {
            var expectedStanza = Presence.New();

            await VerifySendAsyncMethod((s, e) => s.SendPresenceStatusAsync(true), expectedStanza);
        }

        [Fact]
        public async Task SendPresenceStatusAsync_Sends_Correct_Stanza_For_Unavailable_State()
        {
            var expectedStanza = Presence.New().Type("unavailable");

            await VerifySendAsyncMethod((s, e) => s.SendPresenceStatusAsync(false), expectedStanza);
        }

        public static IEnumerable<object[]> GetTestDataForSendPresenceStatus()
        {
            yield return new object[] { true, AvailabilitySubstate.Away, Presence.New().ShowAway() };
            yield return new object[] { true, AvailabilitySubstate.Chat, Presence.New().ShowChat() };
            yield return new object[] { true, AvailabilitySubstate.DoNotDisturb, Presence.New().ShowDoNotDisturb() };
            yield return new object[] { true, AvailabilitySubstate.ExtendedAway, Presence.New().ShowExtendedAway() };
        }

        [Theory]
        [MemberData(nameof(GetTestDataForSendPresenceStatus))]
        public void SendPresenceStatus_Sends_Correct_Stanza_For_Substatus(
            bool isAvailable,
            AvailabilitySubstate substate,
            XmlElement expectedStanza)
        {
            VerifySendMethod((s, e) => s.SendPresenceStatus(isAvailable, substate), expectedStanza);
        }

        [Theory]
        [MemberData(nameof(GetTestDataForSendPresenceStatus))]
        public async Task SendPresenceStatusAsync_Sends_Correct_Stanza_For_Substatus(
            bool isAvailable,
            AvailabilitySubstate substate,
            XmlElement expectedStanza)
        {
            await VerifySendAsyncMethod((s, e) => s.SendPresenceStatusAsync(isAvailable, substate), expectedStanza);
        }

        [Fact]
        public void SendPresenceProbe_Sends_Correct_Stanza()
        {
            Jid from = "user1@domain";
            Jid to = "user2@domain";

            var expectedStanza = Presence.New()
                .From(from)
                .To(to)
                .Type("probe");

            VerifySendMethod((s, e) => s.SendPresenceProbe(from, to), expectedStanza);
        }

        [Fact]
        public async Task SendPresenceProbeAsync_Sends_Correct_Stanza()
        {
            Jid from = "user1@domain";
            Jid to = "user2@domain";

            var expectedStanza = Presence.New()
                .From(from)
                .To(to)
                .Type("probe");

            await VerifySendAsyncMethod((s, e) => s.SendPresenceProbeAsync(from, to), expectedStanza);
        }
    }
}
