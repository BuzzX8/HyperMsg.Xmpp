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
            var stanza = CreateStatusUpdateStanza(presenceStatus);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        private XmlElement CreateStatusUpdateStanza(PresenceStatus presenceStatus)
        {
            return new XmlElement("presence")
                .From(jid)
                .Show(presenceStatus.AvailabilitySubstate.ToString().ToLower())
                .Status(presenceStatus.StatusText);
        }

        public void Handle(XmlElement presenceStanza)
        {
            var status = ToPresenceStatus(presenceStanza);
            StatusUpdateReceived?.Invoke(status);
        }

        private PresenceStatus ToPresenceStatus(XmlElement presenceStanza)
        {
            Enum.TryParse<AvailabilitySubstate>(presenceStanza.Child("show").Value, true, out var substate);
            return new PresenceStatus
            {
                AvailabilitySubstate = substate,
                StatusText = presenceStanza.Child("status").Value
            };
        }

        private static void SetPresenceSubstate(XmlElement stanza, AvailabilitySubstate substate)
        {
            var showItem = new XmlElement("show");

            switch (substate)
            {
                case AvailabilitySubstate.Away:
                    showItem.Value = PresenceStanza.ShowStatus.Away;
                    break;

                case AvailabilitySubstate.Chat:
                    showItem.Value = PresenceStanza.ShowStatus.Chat;
                    break;

                case AvailabilitySubstate.DoNotDisturb:
                    showItem.Value = PresenceStanza.ShowStatus.DoNotDisturb;
                    break;

                case AvailabilitySubstate.ExtendedAway:
                    showItem.Value = PresenceStanza.ShowStatus.ExtendedAway;
                    break;

                default:
                    throw new NotSupportedException();
            }

            stanza.Children.Add(showItem);
        }

        public event Action<PresenceStatus> StatusUpdateReceived;
    }
}
