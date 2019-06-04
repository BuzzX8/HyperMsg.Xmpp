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
        /// </returns>
        Task<bool> NegotiateAsync(ITransceiver<XmlElement, XmlElement> transciever, XmlElement featureElement);
    }
}
