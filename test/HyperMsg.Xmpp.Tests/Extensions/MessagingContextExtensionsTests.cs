﻿using HyperMsg.Xmpp.Xml;
using HyperMsg.Xmpp.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using HyperMsg.Extensions;
using System.Linq;

namespace HyperMsg.Xmpp.Tests.Extensions
{
    public class MessagingContextExtensionsTests
    {
        private readonly Host host;
        private readonly IMessagingContext messagingContext;
        private readonly XmppConnectionSettings settings;
        private readonly CancellationTokenSource tokenSource;
        private readonly List<XmlElement> sentElements;

        private readonly Jid jid = "user@domain.com";

        public MessagingContextExtensionsTests()
        {
            var services = new ServiceCollection();
            services.AddMessagingServices();
            host = new Host(services);
            host.StartAsync().Wait();

            messagingContext = host.Services.GetRequiredService<IMessagingContext>();
            messagingContext.Observable.OnTransmit<XmlElement>(e => sentElements.Add(e));
            sentElements = new List<XmlElement>();
            settings = new XmppConnectionSettings(jid);
            tokenSource = new CancellationTokenSource();
        }

        #region OpenStreamAsync

        [Fact]
        public async Task OpenStreamAsync_Sends_StreamHeader()
        {
            await messagingContext.OpenStreamAsync(settings, tokenSource.Token);

            VerifyStreamHeader(sentElements.Single());
        }

        private void VerifyStreamHeader(XmlElement element)
        {
            Assert.Equal(jid.Domain, element["to"]);
            Assert.Equal("stream:stream", element.Name);
            Assert.Equal(XmppNamespaces.JabberClient, element["xmlns"]);
            Assert.Equal(XmppNamespaces.Streams, element["xmlns:stream"]);
        }

        [Fact]
        public async Task OpenStreamAsync_Throws_Exception_If_Invalid_Header_Received()
        {
            await messagingContext.OpenStreamAsync(settings, tokenSource.Token);

            var incorrectHeader = new XmlElement("stream:stream1").Xmlns(XmppNamespaces.JabberServer);
            await Assert.ThrowsAsync<XmppException>(() => messagingContext.Sender.ReceivedAsync(incorrectHeader, tokenSource.Token));
        }

        [Fact]
        public async Task OpenStreamAsync_Does_Not_Throws_Exception_If_Correct_Header_Received()
        {
            await messagingContext.OpenStreamAsync(settings, tokenSource.Token);
            var streamHeader = CreateStreamHeaderResponse();

            await messagingContext.Sender.ReceivedAsync(streamHeader, tokenSource.Token);
        }

        [Fact]
        public async Task OpenStreamAsync_Throws_Exception_If_Incorrect_Features_Response_Received()
        {
            await SetWaitingFeaturesStateAsync();

            var features = new XmlElement("stream:incorrect-features");
            await Assert.ThrowsAsync<XmppException>(() => messagingContext.Sender.ReceivedAsync(features, tokenSource.Token));
        }

        private async Task SetWaitingFeaturesStateAsync()
        {
            await messagingContext.OpenStreamAsync(settings, tokenSource.Token);
            var streamHeader = CreateStreamHeaderResponse();
            await messagingContext.Sender.ReceivedAsync(streamHeader, tokenSource.Token);
        }

        private XmlElement CreateStreamHeaderResponse() => StreamHeader.Server().From(jid.Domain);

        #endregion
    }
}