using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Represents feature negotiator which is used to bind resource during stream negotiation.
    /// </summary>
    internal class BindNegotiator : FeatureNegotiator
    {
        public BindNegotiator(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        public string Resource { get; set; }

        protected override bool CanNegotiate(XmlElement feature) => feature.Name == "bind" && feature.Xmlns() == XmppNamespaces.Bind;

        protected override Task SendNegotiationRequestAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);

            var request = CreateBindRequest(Resource);
            return this.SendToTransmitPipeAsync(request, cancellationToken);
        }

        protected override Task HandleResponseAsync(XmlElement response, CancellationToken cancellationToken)
        {
            if (!IsBindResponse(response))
            {
                throw new XmppException();
            }

            var boundJid = GetJidFromBind(response);
            
            SetNegotiationCompleted(false);
            return Task.CompletedTask;
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
