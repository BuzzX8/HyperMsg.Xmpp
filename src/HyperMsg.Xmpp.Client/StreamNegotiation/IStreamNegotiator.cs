using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Represents an initializer whose purpose is to negotiate XMPP stream after opening connection.
    /// </summary>
    public interface IStreamNegotiator
    {
        /// <summary>
        /// Negotiates XMPP stream.
        /// </summary>
        /// <param name="channel">
        /// XMPP channel.
        /// </param>
        /// <param name="settings">
        /// Connection settings for XMPP stream.
        /// </param>
        /// <returns>
        /// Result of negotiation.
        /// </returns>
        StreamNegotiationResult Negotiate(ITransceiver<XmlElement, XmlElement> transceiver, XmppConnectionSettings settings);

        /// <summary>
        /// Asynchronously negotiates XMPP stream.
        /// </summary>
        /// <param name="channel">
        /// XMPP channel.
        /// </param>
        /// <param name="settings">
        /// Connection settings for XMPP stream.
        /// </param>
        /// <returns>
        /// Task that represents asyncronous operation of XMPP stream negotiation.
        /// </returns>
        Task<StreamNegotiationResult> NegotiateAsync(ITransceiver<XmlElement, XmlElement> channel, XmppConnectionSettings settings);
    }
}
