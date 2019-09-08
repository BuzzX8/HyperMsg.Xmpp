using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    /// <summary>
    /// Represents feature negotiator which is used to bind resource during stream negotiation.
    /// </summary>
    public class BindFeatureComponent : IFeatureComponent
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly string resource;

        public BindFeatureComponent(IMessageSender<XmlElement> messageSender, string resource)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public bool CanNegotiate(XmlElement feature) => feature.Name == "bind" && feature.Xmlns() == XmppNamespaces.Bind;

        public async Task<FeatureNegotiationState> StartNegotiationAsync(XmlElement featureElement, CancellationToken cancellationToken)
        {
            VerifyFeature(featureElement);
            var bindRequest = CreateBindRequest();
            await messageSender.SendAsync(bindRequest, cancellationToken);
            return FeatureNegotiationState.Negotiating;
        }

        public Task<FeatureNegotiationState> HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            if (!IsBindResponse(element))
            {
                throw new XmppException();
            }

            var boundJid = GetJidFromBind(element);
            JidBound?.Invoke(boundJid);

            return Task.FromResult(FeatureNegotiationState.Completed);
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
            bindResponse.ThrowIfStanzaError(Resources.BindErrorReceived);

            if (!bindResponse.IsIqStanza()
                || !bindResponse.HasChild("bind")
                || !bindResponse.Child("bind").HasChild("jid"))
            {
                throw new XmppException(Resources.InvalidBindResponse);
            }

            return bindResponse
                .Child("bind")
                .Child("jid").Value;
        }

        public event Action<Jid> JidBound;
    }
}
