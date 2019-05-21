using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public class PresenceSubscriptionExtensionTests : SenderExtensionTestBase
    {
        private readonly string to = "user@home";

        [Fact]
        public void SendSubscriptionRequest_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Subscribe().To(to);

            VerifySendMethod((s, e) => s.SendSubscriptionRequest(to), expectedStanza);
        }

        [Fact]
        public async Task SendSubscriptionRequestAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Subscribe().To(to);

            await VerifySendAsyncMethod((s, e) => s.SendSubscriptionRequestAsync(to), expectedStanza);
        }

        [Fact]
        public void SendSubscriptionApprove_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Subscribed().To(to);

            VerifySendMethod((s, e) => s.SendSubscriptionApprove(to), expectedStanza);
        }

        [Fact]
        public async Task SendSubscriptionApproveAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Subscribed().To(to);

            await VerifySendAsyncMethod((s, e) => s.SendSubscriptionApproveAsync(to), expectedStanza);
        }

        [Fact]
        public void SendSubscriptionDeny_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Unsubscribed().To(to);

            VerifySendMethod((s, e) => s.SendSubscriptionDeny(to), expectedStanza);
        }

        [Fact]
        public async Task SendSubscriptionDenyAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Unsubscribed().To(to);

            await VerifySendAsyncMethod((s, e) => s.SendSubscriptionDenyAsync(to), expectedStanza);
        }

        [Fact]
        public void SendSubscriptionCancellation_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Unsubscribed().To(to);

            VerifySendMethod((s, e) => s.SendSubscriptionCancellation(to), expectedStanza);
        }

        [Fact]
        public async Task SendSubscriptionCancellationAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Unsubscribed().To(to);

            await VerifySendAsyncMethod((s, e) => s.SendSubscriptionCancellationAsync(to), expectedStanza);
        }

        [Fact]
        public void SendUnsubscribeRequest_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Unsubscribe().To(to);

            VerifySendMethod((s, e) => s.SendUnsubscribeRequest(to), expectedStanza);
        }

        [Fact]
        public async Task SendUnsubscribeRequestAsync_Sends_Correct_Presence_Stanza()
        {
            var expectedStanza = Presence.Unsubscribe().To(to);

            await VerifySendAsyncMethod((s, e) => s.SendUnsubscribeRequestAsync(to), expectedStanza);
        }
    }
}
