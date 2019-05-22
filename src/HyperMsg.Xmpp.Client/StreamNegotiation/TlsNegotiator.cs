using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Represents feature negotiator that is used to negotiate TLS over XMPP stream.
    /// </summary>
    public class TlsNegotiator : IFeatureNegotiator
    {
        public string FeatureName => "starttls";

        public bool IsStreamRestartRequired => true;

        public FeatureNegotiationResult Negotiate(ITransceiver<XmlElement, XmlElement> channel, XmlElement feature)
        {
            VerifyFeature(feature);
            channel.Send(Tls.Start());
            var response = channel.ReceiveNoStreamError();
            OnResponseReceived(response);
            //GetTlsContext().SetTlsStream();

            return new FeatureNegotiationResult(true);
        }

        public async Task<FeatureNegotiationResult> NegotiateAsync(ITransceiver<XmlElement, XmlElement> channel, XmlElement feature)
        {
            VerifyFeature(feature);
            await channel.SendAsync(Tls.Start(), CancellationToken.None);
            var response = await channel.ReceiveNoStreamErrorAsync();
            OnResponseReceived(response);
            //await GetTlsContext().SetTlsStreamAsync(CancellationToken.None);

            return new FeatureNegotiationResult(true);
        }

        private void VerifyFeature(XmlElement element)
        {
            if (element.Name != "starttls" && element.Xmlns() != XmppNamespaces.Tls)
            {
                throw new XmppException(Resources.InvalidTlsFeature);
            }
        }

        private void OnResponseReceived(XmlElement response)
        {
            if (response.Xmlns() == XmppNamespaces.Tls && response.Name == "failure")
            {
                throw new XmppException(Resources.TlsFailureReceived);
            }

            if (response.Xmlns() != XmppNamespaces.Tls && response.Name != "starttls")
            {
                throw new XmppException(Resources.InvalidTlsResponseReceived);
            }
        }

        //IClientTlsContext GetTlsContext()
        //{
        //    var tlsContext = context.GetService(typeof(IClientTlsContext)) as IClientTlsContext;

        //    if (tlsContext == null)
        //    {
        //        throw new NotSupportedException(Resources.ChannelDoesNotSecure);
        //    }
        //    return tlsContext;
        //}
    }
}
