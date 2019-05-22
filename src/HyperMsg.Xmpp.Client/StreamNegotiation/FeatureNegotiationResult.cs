using System.Collections.Generic;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Result of feature negotiation.
    /// </summary>
    public class FeatureNegotiationResult
    {
        private Dictionary<string, object> data;

        public FeatureNegotiationResult(bool streamRestartRequired)
        {
            IsStreamRestartRequired = streamRestartRequired;
        }

        /// <summary>
        /// Returns arbitrary data related to negotiated feature.
        /// </summary>
        public IDictionary<string, object> Data => data ?? (data = new Dictionary<string, object>());

        /// <summary>
        /// Returns true if XMPP stream must be restarted after after feature negotiation.
        /// </summary>
        public bool IsStreamRestartRequired { get; }
    }

    public static class ResultData
    {
        public static readonly string BoundJid = "Bind.Jid";
    }
}
