using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class PresenceService : IPresenceService
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly Jid jid;

        public PresenceService(IMessageSender<XmlElement> messageSender, Jid jid)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.jid = jid ?? throw new ArgumentNullException(nameof(jid));
        }

        public Task UpdateStatusAsync(PresenceStatus presenceStatus, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public event Action<PresenceStatus> StatusUpdateReceived;
    }
}
