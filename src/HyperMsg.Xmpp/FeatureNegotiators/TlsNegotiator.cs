using HyperMsg.Extensions;
using HyperMsg.Transport;
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
        
        public bool CanNegotiate(XmlElement feature) => StartTls.Equals(feature);

        public Task<MessagingTask<bool>> NegotiateAsync(IMessagingContext messagingContext, XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            return new TlsTask(messagingContext, cancellationToken).StartAsync();
        }

        private void VerifyFeature(XmlElement tlsFeature)
        {
            if (!CanNegotiate(tlsFeature))
            {
                throw new XmppException("InvalidTlsFeature");
            }
        }

        private class TlsTask : MessagingTask<bool>
        {
            public TlsTask(IMessagingContext messagingContext, CancellationToken cancellationToken) : base(messagingContext, cancellationToken)
            {
                AddReceiver<XmlElement>(HandleAsync);
            }

            public async Task<MessagingTask<bool>> StartAsync()
            {
                await TransmitAsync(StartTls, CancellationToken);
                return this;
            }

            private async Task HandleAsync(XmlElement response, CancellationToken cancellationToken)
            {
                VerifyResponse(response);
                await SendAsync(TransportCommand.SetTransportLevelSecurity, cancellationToken);
                Complete(false);
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
}
