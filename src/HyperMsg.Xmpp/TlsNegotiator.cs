using HyperMsg.Transport;
using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Represents feature negotiator that is used to negotiate TLS over XMPP stream.
    /// </summary>
    public class TlsNegotiator : FeatureNegotiator
    {
        private static readonly XmlElement StartTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);

        public TlsNegotiator(IMessagingContext messagingContext) : base(messagingContext)
        { }

        protected override bool CanNegotiate(XmlElement feature) => StartTls.Equals(feature);

        protected override Task SendNegotiationRequestAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            return this.SendToTransmitPipeAsync(StartTls, cancellationToken);
        }

        protected override async Task HandleResponseAsync(XmlElement response, CancellationToken cancellationToken)
        {
            VerifyResponse(response);
            await this.SendTransportMessageAsync(TransportMessage.SetTls, cancellationToken);
            SetNegotiationCompleted(true);
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

            if (response.Xmlns() != XmppNamespaces.Tls && response.Name != "proceed")
            {
                throw new XmppException("InvalidTlsResponseReceived");
            }
        }
    }
}
