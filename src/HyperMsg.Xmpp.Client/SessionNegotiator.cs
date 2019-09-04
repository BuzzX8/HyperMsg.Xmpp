﻿using HyperMsg.Xmpp.Client.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    /// <summary>
	/// Represents negotiator that establishes session.
	/// </summary>
	public class SessionNegotiator : IFeatureComponent
    {
        public string FeatureName => "session";

        public async Task<bool> NegotiateAsync(XmlElement featureElement, CancellationToken cancellationToken)
        {
            VerifyFeature(featureElement);
            var request = CreateSessionRequest();
            //await channel.SendAsync(request, CancellationToken.None);
            //var response = await channel.ReceiveNoStreamErrorAsync();
            //VerifyResponse(response);

            return false;
        }

        private void VerifyFeature(XmlElement feature)
        {
            if (feature.Name != "session" && feature.Xmlns() != XmppNamespaces.Session)
            {
                throw new XmppException(Resources.InvalidSessionFeature);
            }
        }

        private XmlElement CreateSessionRequest()
        {
            return IqStanza.Set()
                .NewId()
                .Children(new XmlElement("session")
                    .Xmlns(XmppNamespaces.Session));
        }

        private void VerifyResponse(XmlElement response)
        {
            response.ThrowIfStanzaError(Resources.SessionErrorReceived);

            //if (!response.IsIq() && !response.IsResult())
            //{
            //    throw new XmppException(Resources.InvalidSessionResponseReceived);
            //}
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
    }
}