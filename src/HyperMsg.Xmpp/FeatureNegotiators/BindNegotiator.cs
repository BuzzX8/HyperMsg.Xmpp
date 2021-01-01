using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.FeatureNegotiators
{
    /// <summary>
    /// Represents feature negotiator which is used to bind resource during stream negotiation.
    /// </summary>
    public class BindNegotiator : IFeatureNegotiator
    {        
        public string Resource { get; set; }

        public bool CanNegotiate(XmlElement feature) => feature.Name == "bind" && feature.Xmlns() == XmppNamespaces.Bind;

        public Task<MessagingTask<bool>> NegotiateAsync(IMessagingContext messagingContext, XmlElement featureElement, CancellationToken cancellationToken)
        {
            VerifyFeature(featureElement);

            return new BindTask(messagingContext, cancellationToken).StartAsync(Resource);
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException();
            }
        }

        private class BindTask : MessagingTask<bool>
        {
            public BindTask(IMessagingContext messagingContext, CancellationToken cancellationToken = default) : base(messagingContext, cancellationToken)
            {
                AddReceiver<XmlElement>(Handle);
            }

            public async Task<MessagingTask<bool>> StartAsync(string resource)
            {
                var bindRequest = CreateBindRequest(resource);
                await TransmitAsync(bindRequest, CancellationToken);

                return this;
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

            private void Handle(XmlElement element)
            {
                if (!IsBindResponse(element))
                {
                    throw new XmppException();
                }

                var boundJid = GetJidFromBind(element);

                Complete(false);
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
}
