using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Components
{
    public class PresenceSubscriptionComponentTests
    {
        private readonly IMessageSender messageSender;
        private readonly PresenceSubscriptionComponent service;
        private readonly CancellationToken cancellationToken;

        public PresenceSubscriptionComponentTests()
        {
            messageSender = A.Fake<IMessageSender>();
            service = new PresenceSubscriptionComponent(messageSender);
            cancellationToken = new CancellationToken();
        }

        [Fact]
        public async Task ApproveSubscriptionAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Subscribed).To(subscriptionJid);

            await service.ApproveSubscriptionAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task CancelSubscriptionAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribed).To(subscriptionJid);

            await service.CancelSubscriptionAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task RequestSubscriptionAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Subscribe).To(subscriptionJid);

            await service.RequestSubscriptionAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task UnsubscribeAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribe).To(subscriptionJid);

            await service.UnsubscribeAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public void Handle_Rises_SubscriptionRequested_For_Subscribe_Presence_Type()
        {
            var expectedJid = $"{Guid.NewGuid()}@domain.com";
            var actualJid = default(Jid);
            service.SubscriptionRequested += jid => actualJid = jid;
            var stanza = PresenceStanza.New(PresenceStanza.Type.Subscribe).From(expectedJid);

            service.Handle(stanza);

            Assert.Equal(expectedJid, actualJid);
        }

        [Fact]
        public void Handle_Rises_SubscriptionApproved_For_Subscribed_Presence_Type()
        {
            var expectedJid = $"{Guid.NewGuid()}@domain.com";
            var actualJid = default(Jid);
            service.SubscriptionApproved += jid => actualJid = jid;
            var stanza = PresenceStanza.New(PresenceStanza.Type.Subscribed).From(expectedJid);

            service.Handle(stanza);

            Assert.Equal(expectedJid, actualJid);
        }

        [Fact]
        public void Handle_Rises_SubscriptionCanceled_For_Unsubscribed_Presence_Type()
        {
            var expectedJid = $"{Guid.NewGuid()}@domain.com";
            var actualJid = default(Jid);
            service.SubscriptionCanceled += jid => actualJid = jid;
            var stanza = PresenceStanza.New(PresenceStanza.Type.Unsubscribed).From(expectedJid);

            service.Handle(stanza);

            Assert.Equal(expectedJid, actualJid);
        }
    }
}
