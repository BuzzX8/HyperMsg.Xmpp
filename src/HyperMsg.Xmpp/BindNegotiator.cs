using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Represents feature negotiator which is used to bind resource during stream negotiation.
    /// </summary>
    public class BindNegotiator : MessagingService
    {
        public BindNegotiator(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        public string Resource { get; set; }

        public bool CanNegotiate(XmlElement feature) => feature.Name == "bind" && feature.Xmlns() == XmppNamespaces.Bind;

        public Task SendNegotiationRequestAsync(IMessageSender messageSender, XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);

            var request = CreateBindRequest(Resource);
            return messageSender.SendToTransmitPipeAsync(request, cancellationToken);
        }

        public Task<(bool IsNegotiationCompleted, bool IsStreamRestartRequired)> HandleResponseAsync(XmlElement response, CancellationToken cancellationToken)
        {
            if (!IsBindResponse(response))
            {
                throw new XmppException();
            }

            var boundJid = GetJidFromBind(response);
            
            return Task.FromResult((true, false));
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException();
            }
        }

        private XmlElement CreateBindRequest(string resource)
        {
            var bindIq = IqStanza.Set().NewId();
            var bind = new XmlElement("bind").Xmlns(XmppNamespaces.Bind);
            bindIq.Children(bind);

            if (!string.IsNullOrEmpty(resource))
            {
                bind.Children(new XmlElement("resource")
                {
                    Value = resource
                });
            }

            return bindIq;
        }

        private bool IsBindResponse(XmlElement response)
        {
            return response.IsIqStanza()
                && response.IsType(IqStanza.Type.Result)
                && response.HasChild("bind");
        }

        private Jid GetJidFromBind(XmlElement bindResponse)
        {
            bindResponse.ThrowIfStanzaError("BindErrorReceived");

            if (!bindResponse.IsIqStanza()
                || !bindResponse.HasChild("bind")
                || !bindResponse.Child("bind").HasChild("jid"))
            {
                throw new XmppException("InvalidBindResponse");
            }

            return bindResponse
                .Child("bind")
                .Child("jid").Value;
        }
    }
}
