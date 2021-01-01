using HyperMsg.Extensions;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.FeatureNegotiators
{
    /// <summary>
    /// Represents feature negotiator which is used to bind resource during stream negotiation.
    /// </summary>
    public class BindNegotiator : IFeatureNegotiator
    {        
        private readonly string resource;

        public BindNegotiator()
        { }

        public bool CanNegotiate(XmlElement feature) => feature.Name == "bind" && feature.Xmlns() == XmppNamespaces.Bind;

        public async Task<MessagingTask<bool>> NegotiateAsync(IMessagingContext messagingContext, XmlElement featureElement, CancellationToken cancellationToken)
        {
            VerifyFeature(featureElement);
            var bindRequest = CreateBindRequest();
            //await messagingContext.Sender.TransmitAsync(bindRequest, cancellationToken);
            return null;// false;
        }

        private Task HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            if (!IsBindResponse(element))
            {
                throw new XmppException();
            }

            var boundJid = GetJidFromBind(element);

            return Task.CompletedTask;
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException();
            }
        }

        private XmlElement CreateBindRequest()
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
