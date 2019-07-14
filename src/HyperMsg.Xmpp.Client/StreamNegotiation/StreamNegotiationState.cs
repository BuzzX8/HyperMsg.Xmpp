﻿namespace HyperMsg.Xmpp.Client.StreamNegotiation
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