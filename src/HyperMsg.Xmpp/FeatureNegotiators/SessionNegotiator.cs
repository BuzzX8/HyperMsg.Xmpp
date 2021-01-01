using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.FeatureNegotiators
{
    /// <summary>
	/// Represents negotiator that establishes session.
	/// </summary>
	public class SessionNegotiator : IFeatureNegotiator
    {
        public bool CanNegotiate(XmlElement feature) => feature.Name == "session" && feature.Xmlns() == XmppNamespaces.Session;

        public Task<MessagingTask<bool>> NegotiateAsync(IMessagingContext messagingContext, XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);

            return new SessionTask(messagingContext, cancellationToken).StartAsync();
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException("InvalidSessionFeature");
            }
        }

        private class SessionTask : MessagingTask<bool>
        {
            public SessionTask(IMessagingContext messagingContext, CancellationToken cancellationToken = default) : base(messagingContext, cancellationToken)
            {
                AddReceiver<XmlElement>(Handle);
            }

            public async Task<MessagingTask<bool>> StartAsync()
            {
                var request = CreateSessionRequest();
                await TransmitAsync(request, CancellationToken);

                return this;
            }

            private void Handle(XmlElement element)
            {
                VerifyResponse(element);
                Complete(true);
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
}
