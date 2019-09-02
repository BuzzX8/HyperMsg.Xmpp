namespace HyperMsg.Xmpp.Client
{
    public enum StreamNegotiationState
    {
        None,
        WaitingStreamHeader,
        WaitingStreamFeatures,
        NegotiatingFeature,
        Done
    }
}