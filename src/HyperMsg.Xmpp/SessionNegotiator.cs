using HyperMsg.Xmpp.Xml;

namespace HyperMsg.Xmpp
{
    /// <summary>
	/// Represents negotiator that establishes session.
	/// </summary>
	public class SessionNegotiator : MessagingService
    {
        public SessionNegotiator(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        public bool CanNegotiate(XmlElement feature) => feature.Name == "session" && feature.Xmlns() == XmppNamespaces.Session;

        

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
