﻿using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    /// <summary>
	/// Represents negotiator that establishes session.
	/// </summary>
	public class SessionNegotiator : IFeatureNegotiator
    {
        public string FeatureName => "session";

        public FeatureNegotiationResult Negotiate(ITransceiver<XmlElement, XmlElement> channel, XmlElement featureElement)
        {
            VerifyFeature(featureElement);
            var request = CreateSessionRequest();
            channel.Send(request);
            var response = channel.ReceiveNoStreamError();
            VerifyResponse(response);

            return new FeatureNegotiationResult(false);
        }

        public async Task<FeatureNegotiationResult> NegotiateAsync(ITransceiver<XmlElement, XmlElement> channel, XmlElement featureElement)
        {
            VerifyFeature(featureElement);
            var request = CreateSessionRequest();
            await channel.SendAsync(request, CancellationToken.None);
            var response = await channel.ReceiveNoStreamErrorAsync();
            VerifyResponse(response);

            return new FeatureNegotiationResult(false);
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
            return Iq.Set()
                .NewId()
                .Children(new XmlElement("session")
                    .Xmlns(XmppNamespaces.Session));
        }

        private void VerifyResponse(XmlElement response)
        {
            response.ThrowIfStanzaError(Resources.SessionErrorReceived);

            if (!response.IsIq() && !response.IsResult())
            {
                throw new XmppException(Resources.InvalidSessionResponseReceived);
            }
        }
    }
}