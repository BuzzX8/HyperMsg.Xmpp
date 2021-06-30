using HyperMsg.Xmpp.Xml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp
{
    public class BindNegotiatorTests : ServiceHostFixture
    {
        private readonly XmlElement bindFeature = new XmlElement("bind").Xmlns(XmppNamespaces.Bind);

        public BindNegotiatorTests()
        {
        }

        //[Fact]
        //public async Task StartNegotiationAsync_Throws_Exception_If_Invalid_Feature_Provided()
        //{
        //    var feature = new XmlElement("invalid-feature");

        //    await Assert.ThrowsAsync<XmppException>(() => negotiator.NegotiateAsync(messagingContext, feature, tokenSource.Token));
        //}

        //[Fact]
        //public async Task NegotiateAsync_Sends_Empty_Bind_If_No_Resource_Provided()
        //{
        //    var bindRequest = default(XmlElement);
        //    messagingContext.Observable.OnTransmit<XmlElement>(e => bindRequest = e);
        //    await negotiator.NegotiateAsync(messagingContext, bindFeature, tokenSource.Token);
                        
        //    VerifyBindRequest(bindRequest, string.Empty);
        //}

        //[Fact]
        //public async Task NegotiateAsync_Sends_Non_Empty_Bind_If_Resource_Provided()
        //{
        //    var resource = Guid.NewGuid().ToString();
        //    negotiator.Resource = resource;
        //    var actualStanza = default(XmlElement);
        //    messagingContext.Observable.OnTransmit<XmlElement>(e => actualStanza = e);

        //    await negotiator.NegotiateAsync(messagingContext, bindFeature, tokenSource.Token);

        //    VerifyBindRequest(actualStanza, resource);
        //}

        //private void VerifyBindRequest(XmlElement bindRequest, string resource)
        //{
        //    Assert.Equal("iq", bindRequest.Name);
        //    Assert.Equal("set", bindRequest["type"]);
        //    var bind = bindRequest.Children.Single();
        //    Assert.Equal("bind", bind.Name);
        //    Assert.Equal(XmppNamespaces.Bind, bind.Xmlns());

        //    if (!string.IsNullOrEmpty(resource))
        //    {
        //        var resourceElement = bind.Children.Single();
        //        Assert.Equal("resource", resourceElement.Name);
        //    }
        //}

        //[Fact]
        //public async Task Throws_Exception_If_Receives_Invalid_Bind_Response()
        //{
        //    await negotiator.NegotiateAsync(messagingContext, bindFeature, tokenSource.Token);
        //    var response = new XmlElement("invalid-response");

        //    await Assert.ThrowsAsync<XmppException>(() => messagingContext.Sender.ReceivedAsync(response, tokenSource.Token));
        //}

        //[Fact]
        //public async Task Throws_Exception_If_Bind_Error_Received()
        //{
        //    await negotiator.NegotiateAsync(messagingContext, bindFeature, tokenSource.Token);
        //    var response = IqStanza.Error().Children(new XmlElement("error"));

        //    await Assert.ThrowsAsync<XmppException>(() => messagingContext.Sender.ReceivedAsync(response, tokenSource.Token));
        //}

        //[Fact]
        //public async Task Completes_Messaging_Task_If_Corrent_Bind_Response_Received()
        //{
        //    var resource = Guid.NewGuid().ToString();
        //    var task = await negotiator.NegotiateAsync(messagingContext, bindFeature, tokenSource.Token);
        //    var response = CreateBindResponse(resource);

        //    await messagingContext.Sender.ReceivedAsync(response, tokenSource.Token);

        //    Assert.True(task.IsCompleted);
        //}

        //[Fact]
        //public async Task HandleAsync_Rises_JidBound_If_Correct_Response_Received()
        //{
        //    var expectedJid = (Jid)$"expected@jid.com";
        //    var actualJid = default(Jid);
        //    component.JidBound += jid => actualJid = jid;
        //    await component.StartNegotiationAsync(bindFeature, tokenSource.Token);
        //    var response = CreateBindResponse(string.Empty, expectedJid);

        //    await component.HandleAsync(response, tokenSource.Token);

        //    Assert.Equal(expectedJid, actualJid);
        //}

        private XmlElement CreateBindResponse(string resource, string jid = "user@domain")
        {
            return IqStanza.Result().Children(
                new XmlElement("bind").Xmlns(XmppNamespaces.Bind).Children(
                    new XmlElement("jid").Value($"{jid}/{resource}")));
        }
    }
}
