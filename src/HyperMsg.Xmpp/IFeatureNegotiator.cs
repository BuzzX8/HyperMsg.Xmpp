using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Represents initializer which negotiate specific XMPP feature during stream negotiation.
    /// </summary>
    public interface IFeatureNegotiator
    {
        bool CanNegotiate(XmlElement feature);

        Task<MessagingTask<bool>> NegotiateAsync(IMessagingContext messagingContext, XmlElement feature, CancellationToken cancellationToken);
    }
}