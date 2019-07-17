namespace HyperMsg.Xmpp.Client
{
    public class PresenceStatus
    {
        public bool IsAvailable { get; set; }

        public AvailabilitySubstate AvailabilitySubstate { get; set; }

        public string StatusText { get; set; }
    }
}
