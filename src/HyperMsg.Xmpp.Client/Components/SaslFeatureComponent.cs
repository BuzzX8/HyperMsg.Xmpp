using HyperMsg.Xmpp.Client.Properties;
using HyperMsg.Xmpp.Extensions;
using HyperMsg.Xmpp.Xml;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    /// <summary>
    /// Represents authentication negotiator which is used to authenticate user using SASL protocol.
    /// </summary>
    public class SaslFeatureComponent : IFeatureComponent
    {
        private readonly IMessageSender messageSender;
        
        public SaslFeatureComponent(IMessageSender messageSender)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public bool CanNegotiate(XmlElement feature) => feature.Name != "mechanisms" && feature.Xmlns() != XmppNamespaces.Sasl;

        public Task<FeatureNegotiationState> StartNegotiationAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<FeatureNegotiationState> HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (!CanNegotiate(feature))
            {
                throw new XmppException(Resources.InvalidSaslFeature);
            }
        }

        private string[] GetMechanisms(XmlElement feature)
        {
            return feature.Children.Select(m => m.Value?.ToString())
                .ToArray();
        }
    }
}
