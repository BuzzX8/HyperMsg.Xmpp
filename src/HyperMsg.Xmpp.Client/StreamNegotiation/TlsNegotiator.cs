using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Represents feature negotiator that is used to negotiate TLS over XMPP stream.
    /// </summary>
    public class TlsNegotiator //: IFeatureNegotiator
    {
        private static readonly XmlElement StartTls = new XmlElement("starttls").Xmlns(XmppNamespaces.Tls);
        private readonly AsyncAction<TransportCommand> transportCommandHandler;
        private readonly IMessageSender<XmlElement> messageSender;        

        public TlsNegotiator(AsyncAction<TransportCommand> transportCommandHandler, IMessageSender<XmlElement> messageSender)
        {
            this.transportCommandHandler = transportCommandHandler ?? throw new ArgumentNullException(nameof(transportCommandHandler));
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public string FeatureName => "starttls";

        public bool IsStreamRestartRequired => true;

        public async Task NegotiateAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            await messageSender.SendAsync(StartTls, cancellationToken);            
        }

        public Task HandleAsync(XmlElement response, CancellationToken cancellationToken)
        {
            VerifyResponse(response);

            return transportCommandHandler.Invoke(TransportCommand.SetTransportLevelSecurity, cancellationToken);
        }

        private void VerifyFeature(XmlElement tlsFeature)
        {
            if (!StartTls.Equals(tlsFeature))
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
