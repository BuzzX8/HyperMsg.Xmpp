namespace HyperMsg.Xmpp.Extensions
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