using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Components
{
    /// <summary>
    /// Represents authentication negotiator which is used to authenticate user using SASL protocol.
    /// </summary>
    public class SaslNegotiator : IFeatureComponent
    {
        //private IAuthenticator authenticator;

        //public SaslNegotiator(IAuthenticator authenticator)
        //{
        //    this.authenticator = authenticator;
        //}

        public string FeatureName => "mechanisms";

        public async Task<bool> NegotiateAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            VerifyFeature(feature);
            var mechanisms = GetMechanisms(feature);
            //var saslChannel = new XmppSaslChannel(channel);
            //AuthOutcome outcome = await authenticator.AuthenticateAsync(saslChannel, mechanisms);
            //VerifyOutcome(outcome);

            return true;
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

        public bool CanNegotiate(XmlElement feature)
        {
            throw new System.NotImplementedException();
        }

        public Task<FeatureNegotiationState> StartNegotiationAsync(XmlElement feature, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<FeatureNegotiationState> HandleAsync(XmlElement element, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
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
