using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public delegate Task<FeatureNegotiationState> FeatureMessageHandler(XmlElement element, CancellationToken cancellationToken);
}