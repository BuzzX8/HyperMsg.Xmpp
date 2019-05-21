using System.Collections.Generic;
using System.Linq;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Result of negotiating XMPP stream.
    /// </summary>
    public class StreamNegotiationResult
    {
        private Dictionary<string, object> data;

        public StreamNegotiationResult(IEnumerable<string> negotiatedFeatures)
        {
            NegotiatedFeatures = negotiatedFeatures.ToArray();
        }

        public StreamNegotiationResult(
            IEnumerable<string> negotiatedFeatures,
            IEnumerable<FeatureNegotiationResult> featureResults)
            : this(negotiatedFeatures)
        {
            data = new Dictionary<string, object>();

            foreach (var result in featureResults)
            {
                foreach (var data in result.Data)
                {
                    this.data.Add(data.Key, data.Value);
                }
            }
        }

        /// <summary>
        /// Returns arbitrary data about negotiated stream.
        /// </summary>
        public IReadOnlyDictionary<string, object> Data => data ?? (data = new Dictionary<string, object>());

        /// <summary>
        /// Returns collection of features that has been negotiated.
        /// </summary>
        public string[] NegotiatedFeatures { get; }
    }
}
