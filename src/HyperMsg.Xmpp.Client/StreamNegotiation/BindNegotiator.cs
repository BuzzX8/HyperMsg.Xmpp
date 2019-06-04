using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Represents feature negotiator which is used to bind resource during stream negotiation.
    /// </summary>
    public class BindNegotiator : IFeatureNegotiator
    {
        private readonly string resource;

        public BindNegotiator(string resource = "")
        {
            this.resource = resource;
        }

        public string FeatureName => "bind";

        public async Task<bool> NegotiateAsync(ITransceiver<XmlElement, XmlElement> transceiver, XmlElement featureElement)
        {
            VerifyFeature(featureElement);
            var bindRequest = CreateBindRequest();
            await transceiver.SendAsync(bindRequest, CancellationToken.None);
            var response = await transceiver.ReceiveNoStreamErrorAsync();

            return GetResult(response);
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (feature.Name != "bind" && feature.Xmlns() != XmppNamespaces.Bind)
            {
                throw new XmppException();
            }
        }

        private XmlElement CreateBindRequest()
        {
            var bindIq = Iq.Set().NewId();
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

        private bool GetResult(XmlElement response)
        {
            var boundJid = GetJidFromBind(response);

            return false;
        }

        private Jid GetJidFromBind(XmlElement bindResponse)
        {
            bindResponse.ThrowIfStanzaError(Resources.BindErrorReceived);

            if (!bindResponse.IsIq()
                || !bindResponse.HasChild("bind")
                || !bindResponse.Child("bind").HasChild("jid"))
            {
                throw new XmppException(Resources.InvalidBindResponse);
            }

            return bindResponse
                .Child("bind")
                .Child("jid").Value;
        }
    }
}
