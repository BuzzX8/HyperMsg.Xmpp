using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    /// <summary>
    /// Represents initializer which negotiate specific XMPP feature during stream negotiation.
    /// </summary>
    public interface IFeatureComponent
    {
        bool CanNegotiate(XmlElement feature);

        Task<FeatureNegotiationState> StartNegotiationAsync(XmlElement feature, CancellationToken cancellationToken);

        Task<FeatureNegotiationState> HandleAsync(XmlElement element, CancellationToken cancellationToken);
    }
}