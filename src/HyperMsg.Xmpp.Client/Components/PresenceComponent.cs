using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    public class PresenceComponent : IPresenceService
    {
        private readonly IMessageSender messageSender;

        public PresenceComponent(IMessageSender messageSender)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public Task UpdateStatusAsync(PresenceStatus presenceStatus, CancellationToken cancellationToken)
        {
            var stanza = CreateStatusUpdateStanza(presenceStatus);
            return messageSender.SendAsync(stanza, cancellationToken);
        }

        private XmlElement CreateStatusUpdateStanza(PresenceStatus presenceStatus)
        {
            var stanzaType = presenceStatus.IsAvailable ? string.Empty : PresenceStanza.Type.Unavailable;
            var showStatus = ToShowStatus(presenceStatus.AvailabilitySubstate);

            return PresenceStanza.New(stanzaType, showStatus, presenceStatus.StatusText);
        }

        public void Handle(XmlElement presenceStanza)
        {
            if (!IsPresenceStatusStanza(presenceStanza))
            {
                return;
            }

            var entityJid = Jid.Parse(presenceStanza["from"]);
            var status = ToPresenceStatus(presenceStanza);
            StatusUpdated?.Invoke(new PresenceUpdatedEventArgs(entityJid, status));
        }

        private bool IsPresenceStatusStanza(XmlElement stanza)
        {
            return stanza.IsPresenceStanza()
                && (stanza.IsType(PresenceStanza.Type.Unavailable) || string.IsNullOrEmpty(stanza["type"]));
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

        private static string ToShowStatus(AvailabilitySubstate substate)
        {
            switch (substate)
            {
                case AvailabilitySubstate.Away:
                    return PresenceStanza.ShowStatus.Away;

                case AvailabilitySubstate.Chat:
                    return PresenceStanza.ShowStatus.Chat;

                case AvailabilitySubstate.DoNotDisturb:
                    return PresenceStanza.ShowStatus.DoNotDisturb;

                case AvailabilitySubstate.ExtendedAway:
                    return PresenceStanza.ShowStatus.ExtendedAway;

                default:
                    throw new NotSupportedException();
            }
        }

        public event Action<PresenceUpdatedEventArgs> StatusUpdated;
    }
}
