using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    /// <summary>
	/// Represents negotiator that establishes session.
	/// </summary>
	public class SessionFeatureComponent : IFeatureComponent
    {
        private readonly IMessageSender<XmlElement> messageSender;

        public SessionFeatureComponent(IMessageSender<XmlElement> messageSender)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public bool CanNegotiate(XmlElement feature) => feature.Name == "session" && feature.Xmlns() == XmppNamespaces.Session;

        public async Task<FeatureNegotiationState> StartNegotiationAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            var request = CreateSessionRequest();
            await messageSender.SendAsync(request, cancellationToken);
            return FeatureNegotiationState.Negotiating;
        }

        public Task<FeatureNegotiationState> HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            VerifyResponse(element);

            return Task.FromResult(FeatureNegotiationState.Completed);
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException(Resources.InvalidSessionFeature);
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
            response.ThrowIfStanzaError(Resources.SessionErrorReceived);

            if (!response.IsIqStanza() && !response.IsType(IqStanza.Type.Result))
            {
                throw new XmppException(Resources.InvalidSessionResponseReceived);
            }
        }
    }
}
