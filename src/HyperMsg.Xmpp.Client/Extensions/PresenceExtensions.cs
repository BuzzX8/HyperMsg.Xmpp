using System;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class PresenceExtensions
    {
        public static string SendPresenceStatus(this ISender<XmlElement> channel, bool isAvailable)
        {
            var statusStanza = CreatePresenceStanza(isAvailable);

            return channel.SendWithNewId(statusStanza);
        }

        public static string SendPresenceStatus(this ISender<XmlElement> channel, bool isAvailable, AvailabilitySubstate substate)
        {
            var statusStanza = Presence.New();
            SetPresenceSubstate(statusStanza, substate);
            return channel.SendWithNewId(statusStanza);
        }

        public static Task<string> SendPresenceStatusAsync(this ISender<XmlElement> channel, bool isAvailable, AvailabilitySubstate substate)
        {
            var statusStanza = Presence.New();
            SetPresenceSubstate(statusStanza, substate);
            return channel.SendWithNewIdAsync(statusStanza);
        }

        public static Task<string> SendPresenceStatusAsync(this ISender<XmlElement> channel, bool isAvailable)
        {
            var statusStanza = CreatePresenceStanza(isAvailable);

            return channel.SendWithNewIdAsync(statusStanza);
        }

        public static string SendPresenceProbe(this ISender<XmlElement> channel, Jid from, Jid to)
        {
            var probeStanza = CreatePresenceProbeStanza(from, to);

            return channel.SendWithNewId(probeStanza);
        }

        public static Task<string> SendPresenceProbeAsync(this ISender<XmlElement> channel, Jid from, Jid to)
        {
            var probeStanza = CreatePresenceProbeStanza(from, to);

            return channel.SendWithNewIdAsync(probeStanza);
        }

        private static XmlElement CreatePresenceStanza(bool isAvailable)
        {
            var statusStanza = Presence.New();

            if (!isAvailable)
            {
                statusStanza.Type(Presence.Type.Unavailable);
            }

            return statusStanza;
        }

        private static XmlElement CreatePresenceProbeStanza(Jid from, Jid to)
        {
            return Presence.Probe()
                .From(from)
                .To(to);
        }

        private static void SetPresenceSubstate(XmlElement stanza, AvailabilitySubstate substate)
        {
            var showItem = new XmlElement("show");

            switch (substate)
            {
                case AvailabilitySubstate.Away:
                    showItem.Value = Presence.ShowStatus.Away;
                    break;

                case AvailabilitySubstate.Chat:
                    showItem.Value = Presence.ShowStatus.Chat;
                    break;

                case AvailabilitySubstate.DoNotDisturb:
                    showItem.Value = Presence.ShowStatus.DoNotDisturb;
                    break;

                case AvailabilitySubstate.ExtendedAway:
                    showItem.Value = Presence.ShowStatus.ExtendedAway;
                    break;

                default:
                    throw new NotSupportedException();
            }

            stanza.Children.Add(showItem);
        }
    }
}

