﻿using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.StreamNegotiation
{
    public class ClientStreamNegotiator
    {
        private readonly IMessageSender<XmlElement> messageSender;
        private readonly XmppConnectionSettings settings;

        private readonly Dictionary<string, FeatureMessageHandler> negotiators;
        private readonly List<string> negotiatedFeatures;

        public ClientStreamNegotiator(IMessageSender<XmlElement> messageSender, XmppConnectionSettings settings)
        {
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings)); 
            negotiators = new Dictionary<string, FeatureMessageHandler>();
            negotiatedFeatures = new List<string>();
        }

        public StreamNegotiationState State { get; private set; }

        public void AddFeatureHandler(string name, FeatureMessageHandler handler)
        {
            negotiators.Add(name, handler);
        }

        public Task HandleTransportEventAsync(TransportEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var header = CreateHeader(settings.Domain);
            messageSender.SendAsync(header, CancellationToken.None).Wait();
            State = StreamNegotiationState.WaitingStreamHeader;
            return Task.CompletedTask;
        }

        public Task HandleAsync(XmlElement element, CancellationToken _)
        {
            switch (State)
            {
                case StreamNegotiationState.WaitingStreamHeader:
                    HandleStreamHeader(element);
                    break;

                case StreamNegotiationState.WaitingFeatures:
                    HandleFeatures(element);
                    break;
            }
            
            return Task.CompletedTask;
        }

        private void HandleStreamHeader(XmlElement streamHeader)
        {
            VerifyHeader(streamHeader);
            State = StreamNegotiationState.WaitingFeatures;
        }

        private void HandleFeatures(XmlElement element)
        {
            VerifyFeatures(element);

            //if (!HasNegotiatorsForFeatures(element.Children))
            //{
            //    return;
            //}

            var feature = SelectFeature(element.Children);
            var negotiator = GetNegotiator(feature);
            negotiator.Invoke(feature, default);
        }

        private async Task NegotiateAsync(CancellationToken cancellationToken)
        {
            VerifySettings(settings);
            IEnumerable<XmlElement> features = null;
            var negotiatedFeatures = new List<string>();
            bool restartRequired = true;
            bool negotiating = true;

            while (negotiating)
            {
                if (restartRequired)
                {
                    //await SendAndReceiveStreamHeaderAsync(transceiver, settings, cancellationToken);
                    //features = await ReceiveFeaturesAsync(transceiver, cancellationToken);
                }

                if (negotiating = HasNegotiatorsForFeatures(features))
                {
                    var feature = SelectFeature(features);
                    var negotiator = GetNegotiator(feature);
                    //var result = await negotiator.NegotiateAsync(transceiver, feature, cancellationToken);
                    //negotiatedFeatures.Add(negotiator.FeatureName);
                    //restartRequired = result;
                }
            }
        }

        private async Task SendAndReceiveStreamHeaderAsync(XmppConnectionSettings settings, CancellationToken cancellationToken)
        {
            var header = CreateHeader(settings.Domain);
            //await transceiver.SendAsync(header, cancellationToken);
            //var response = await transceiver.ReceiveNoStreamErrorAsync(cancellationToken);
            //VerifyHeader(response);
        }

        private XmlElement CreateHeader(string domain) => StreamHeader.Client().To(domain).NewId();

        private void VerifyHeader(XmlElement header)
        {
            if (!IsStreamHeader(header))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:stream", header.Name));
            }
        }

        private void VerifyFeatures(XmlElement features)
        {
            if (!IsStreamFeatures(features))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:stream", features.Name));
            }
        }

        private bool IsStreamHeader(XmlElement element) => element.Name == "stream:stream";

        private bool IsStreamFeatures(XmlElement element) => element.Name == "stream:features";

        private async Task<IEnumerable<XmlElement>> ReceiveFeaturesAsync(CancellationToken cancellationToken)
        {
            //var features = await transceiver.ReceiveNoStreamErrorAsync(cancellationToken);

            return null;// GetFeatureItems(features);
        }

        private IEnumerable<XmlElement> GetFeatureItems(XmlElement features)
        {
            if (!IsFeatures(features))
            {
                throw new XmppException(string.Format(Resources.InvalidXmlElementReceived, "stream:features", features.Name));
            }

            return features.Children;
        }

        private bool IsFeatures(XmlElement element) => element.Name == "stream:features";

        private FeatureMessageHandler GetNegotiator(XmlElement feature)
        {
            if (!negotiators.ContainsKey(feature.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.NoNegotiatorForFeature, feature.Name));
            }

            return negotiators[feature.Name];
        }

        private bool HasNegotiatorsForFeatures(IEnumerable<XmlElement> features)
        {
            return features.Select(f => f.Name)
                .Except(negotiatedFeatures)
                .Any(f => negotiators.ContainsKey(f));
        }

        private XmlElement SelectFeature(IEnumerable<XmlElement> features)
        {
            if (HasTlsFeature(features)
                && settings.UseTls
                && !negotiatedFeatures.Contains("starttls")
                && negotiators.ContainsKey("starttls"))
            {
                return GetTlsFeature(features);
            }

            if (HasSaslFeature(features)
                && settings.UseSasl
                && !negotiatedFeatures.Contains("mechanisms")
                && negotiators.ContainsKey("mechanisms"))
            {
                return GetSaslFeature(features);
            }

            return features.FirstOrDefault(
                f => negotiators.ContainsKey(f.Name)
                && !negotiatedFeatures.Contains(f.Name));
        }

        private bool HasTlsFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "starttls" && f.Xmlns() == XmppNamespaces.Tls);

        private XmlElement GetTlsFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "starttls");

        private bool HasSaslFeature(IEnumerable<XmlElement> features) => features.Any(f => f.Name == "mechanisms" && f.Xmlns() == XmppNamespaces.Sasl);

        private XmlElement GetSaslFeature(IEnumerable<XmlElement> features) => features.First(f => f.Name == "mechanisms");

        private void VerifySettings(XmppConnectionSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(settings.Domain))
            {
                throw new ArgumentException("Domain is empty.");
            }
        }        
    }
}
