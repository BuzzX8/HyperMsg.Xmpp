using HyperMsg.Extensions;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.FeatureNegotiators
{
    /// <summary>
    /// Represents feature negotiator that is used to negotiate TLS over XMPP stream.
    /// </summary>
    public class TlsNegotiator : IFeatureNegotiator
    {
        private static readonly XmlElement StartTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);

        private TaskCompletionSource<bool> completionSource;

        public bool CanNegotiate(XmlElement feature) => StartTls.Equals(feature);

        public async Task<bool> NegotiateAsync(IMessagingContext messagingContext, XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            await messagingContext.Sender.TransmitAsync(StartTls, cancellationToken);
            return await (completionSource = new TaskCompletionSource<bool>()).Task;
        }

        public async Task HandleAsync(XmlElement response, CancellationToken cancellationToken)
        {
            VerifyResponse(response);                        
            return;
        }

        private void VerifyFeature(XmlElement tlsFeature)
        {
            if (!CanNegotiate(tlsFeature))
            {
                throw new XmppException("InvalidTlsFeature");
            }
        }

        private void VerifyResponse(XmlElement response)
        {
            if (response.Xmlns() == XmppNamespaces.Tls && response.Name == "failure")
            {
                throw new XmppException("TlsFailureReceived");
            }

            if (response.Xmlns() != XmppNamespaces.Tls && response.Name != "starttls")
            {
                throw new XmppException("InvalidTlsResponseReceived");
            }
        }
    }
}
