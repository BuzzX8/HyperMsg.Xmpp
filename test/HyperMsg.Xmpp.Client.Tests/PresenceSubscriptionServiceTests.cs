using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class PresenceSubscriptionServiceTests
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly Jid jid;
        private readonly PresenceSubscriptionService service;

        public PresenceSubscriptionServiceTests()
        {
            messageSender = A.Fake<IMessageSender<XmlElement>>();
            jid = $"{Guid.NewGuid()}@domain.com";
            service = new PresenceSubscriptionService(messageSender, jid);
        }

        [Fact]
        public async Task ApproveSubscriptionAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = new XmlElement("presence")
                .To(subscriptionJid)
                .Type("subscribed");
            var cancellationToken = new CancellationToken();

            await service.ApproveSubscriptionAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task CancelSubscriptionAsync_Sends_Correct_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = new XmlElement("presence")
                .To(subscriptionJid)
                .Type("unsubscribed");
            var cancellationToken = new CancellationToken();

            await service.CancelSubscriptionAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task RequestSubscriptionAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = new XmlElement("presence")
                .To(subscriptionJid)
                .Type("subscribe");
            var cancellationToken = new CancellationToken();

            await service.RequestSubscriptionAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public async Task UnsubscribeAsync_Sends_Correct_Presence_Stanza()
        {
            var subscriptionJid = $"{Guid.NewGuid()}@domain.com";
            var expectedStanza = new XmlElement("presence")
                .To(subscriptionJid)
                .Type("unsubscribe");
            var cancellationToken = new CancellationToken();

            await service.UnsubscribeAsync(subscriptionJid, cancellationToken);

            A.CallTo(() => messageSender.SendAsync(expectedStanza, cancellationToken)).MustHaveHappened();
        }
    }
}
