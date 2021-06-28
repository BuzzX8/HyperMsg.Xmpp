namespace HyperMsg.Xmpp
{
    internal enum StreamNegotiationState
    {
        None,
        WaitingStreamHeader,
        WaitingStreamFeatures,
        NegotiatingFeature,
        Done
    }
}