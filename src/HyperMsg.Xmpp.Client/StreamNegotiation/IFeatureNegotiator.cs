using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Represents initializer which negotiate specific XMPP feature during stream negotiation.
    /// </summary>
    public interface IFeatureNegotiator
    {
        /// <summary>
        /// Name of feature which can be negotiated.
        /// </summary>
        string FeatureName { get; }

        /// <summary>
        /// Negotiates feature.
        /// </summary>
        /// <param name="transceiver">
        /// XMPP channel.
        /// </param>
        /// <param name="featureElement">
        /// XML element which represent specific stream feature. This element usually represented as one of the
        /// child element of stream:features.
        /// </param>
        /// <returns>
        /// Result of feature negotiation.
        /// </returns>
        FeatureNegotiationResult Negotiate(ITransceiver<XmlElement, XmlElement> transceiver, XmlElement featureElement);

        /// <summary>
        /// Asynchronously negotiates feature.
        /// </summary>
        /// <param name="transciever">
        /// XMPP channel.
        /// </param>
        /// <param name="featureElement">
        /// XML element which represent specific stream feature. This element usually represented as one of the
        /// child element of stream:features.
        /// </param>
        /// <returns>
        /// Task which contains result of feature negotiation upon completion.
        /// </returns>
        Task<FeatureNegotiationResult> NegotiateAsync(ITransceiver<XmlElement, XmlElement> transciever, XmlElement featureElement);
    }
}
