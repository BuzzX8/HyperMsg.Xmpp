using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    /// <summary>
	/// Represents negotiator that establishes session.
	/// </summary>
	public class SessionNegotiator : FeatureNegotiator
    {
        public SessionNegotiator(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        protected override bool CanNegotiate(XmlElement feature) => feature.Name == "session" && feature.Xmlns() == XmppNamespaces.Session;

        protected override Task SendNegotiationRequestAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            var request = CreateSessionRequest();
            return this.SendToTransmitPipeAsync(request, cancellationToken);
        }

        protected override Task HandleResponseAsync(XmlElement response, CancellationToken cancellationToken)
        {
            VerifyResponse(response);
            SetNegotiationCompleted(false);
            return Task.CompletedTask;
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException("InvalidSessionFeature");
            }
        }

        private XmlElement CreateSessionRequest()
        {
            return IqStanza.Set()
                .NewId()
                .Children(new XmlElement("session")
                    .Xmlns(XmppNamespaces.Session));
        }

        private void VerifyResponse(XmlElement response)
        {
            response.ThrowIfStanzaError("SessionErrorReceived");

            if (!response.IsIqStanza() && !response.IsType(IqStanza.Type.Result))
            {
                throw new XmppException("InvalidSessionResponseReceived");
            }
        }
    }
}
