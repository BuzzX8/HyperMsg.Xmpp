namespace HyperMsg.Xmpp
{
    public class PresenceStatus
    {
        public Jid Jid { get; set; }

        public bool IsAvailable { get; set; }

        public AvailabilitySubstate AvailabilitySubstate { get; set; }

        public string StatusText { get; set; }
    }
}
