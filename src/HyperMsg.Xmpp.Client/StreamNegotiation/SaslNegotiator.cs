using System.Linq;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
    /// Represents authentication negotiator which is used to authenticate user using SASL protocol.
    /// </summary>
    public class SaslNegotiator : IFeatureNegotiator
    {
        //private IAuthenticator authenticator;

        //public SaslNegotiator(IAuthenticator authenticator)
        //{
        //    this.authenticator = authenticator;
        //}

        public string FeatureName => "mechanisms";

        public bool IsStreamRestartRequired => true;

        public FeatureNegotiationResult Negotiate(ITransceiver<XmlElement, XmlElement> channel, XmlElement feature)
        {
            VerifyFeature(feature);
            var mechanisms = GetMechanisms(feature);
            //var saslChannel = new XmppSaslChannel(channel);
            //AuthOutcome outcome = authenticator.Authenticate(saslChannel, mechanisms);
            //VerifyOutcome(outcome);

            return new FeatureNegotiationResult(true);
        }

        public async Task<FeatureNegotiationResult> NegotiateAsync(ITransceiver<XmlElement, XmlElement> channel, XmlElement feature)
        {
            VerifyFeature(feature);
            var mechanisms = GetMechanisms(feature);
            //var saslChannel = new XmppSaslChannel(channel);
            //AuthOutcome outcome = await authenticator.AuthenticateAsync(saslChannel, mechanisms);
            //VerifyOutcome(outcome);

            return new FeatureNegotiationResult(true);
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (feature.Name != "mechanisms" && feature.Xmlns() != XmppNamespaces.Sasl)
            {
                throw new XmppException();//Resources.InvalidSaslFeature);
            }
        }

        private string[] GetMechanisms(XmlElement feature)
        {
            return feature.Children.Select(m => m.Value?.ToString())
                .ToArray();
        }

        //private void VerifyOutcome(AuthOutcome outcome)
        //{
        //    if (outcome.Result == AuthResult.Fail)
        //    {
        //        throw new AuthorizationException(Resources.SaslAuthFailed);
        //    }

        //    if (outcome.Result == AuthResult.Abort)
        //    {
        //        throw new AuthorizationException(Resources.SaslAuthAbort);
        //    }
        //}
    }
}
