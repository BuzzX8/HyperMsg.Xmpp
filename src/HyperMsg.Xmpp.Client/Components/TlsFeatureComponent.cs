using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    /// <summary>
    /// Represents feature negotiator that is used to negotiate TLS over XMPP stream.
    /// </summary>
    public class TlsFeatureComponent : IFeatureComponent
    {
        private static readonly XmlElement StartTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);
        private readonly AsyncAction<TransportCommand> transportCommandHandler;
        private readonly IMessageSender<XmlElement> messageSender;        

        public TlsFeatureComponent(AsyncAction<TransportCommand> transportCommandHandler, IMessageSender<XmlElement> messageSender)
        {
            this.transportCommandHandler = transportCommandHandler ?? throw new ArgumentNullException(nameof(transportCommandHandler));
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public bool CanNegotiate(XmlElement feature) => StartTls.Equals(feature);

        public async Task<FeatureNegotiationState> StartNegotiationAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            await messageSender.SendAsync(StartTls, cancellationToken);
            return FeatureNegotiationState.Negotiating;
        }

        public async Task<FeatureNegotiationState> HandleAsync(XmlElement response, CancellationToken cancellationToken)
        {
            VerifyResponse(response);

            await transportCommandHandler.Invoke(TransportCommand.SetTransportLevelSecurity, cancellationToken);
            return FeatureNegotiationState.Completed;
        }

        private void VerifyFeature(XmlElement tlsFeature)
        {
            if (!CanNegotiate(tlsFeature))
            {
                throw new XmppException(Resources.InvalidTlsFeature);
            }
        }

        private void VerifyResponse(XmlElement response)
        {
            if (response.Xmlns() == XmppNamespaces.Tls && response.Name == "failure")
            {
                throw new XmppException(Resources.TlsFailureReceived);
            }

            if (response.Xmlns() != XmppNamespaces.Tls && response.Name != "starttls")
            {
                throw new XmppException(Resources.InvalidTlsResponseReceived);
            }
        }
    }
}
