using FakeItEasy;
using System;

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
    }
}
