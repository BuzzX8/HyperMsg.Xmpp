using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public abstract class SenderExtensionTestBase
    {
        private ISender<XmlElement> sender;
        private XmlElement sendedStanza;

        protected XmlElement SendedStanza => sendedStanza;

        public SenderExtensionTestBase()
        {
            sender = A.Fake<ISender<XmlElement>>();
            A.CallTo(() => sender.Send(A<XmlElement>._)).Invokes(foc =>
            {
                sendedStanza = foc.GetArgument<XmlElement>(0);
            });
            A.CallTo(() => sender.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                sendedStanza = foc.GetArgument<XmlElement>(0);
            }).Returns(Task.FromResult(true));
            sendedStanza = null;
        }

        protected void VerifySendMethod(Func<ISender<XmlElement>, XmlElement, string> method, XmlElement expectedStanza)
        {
            var stanzaId = method.Invoke(sender, expectedStanza);

            AreEqual(expectedStanza, sendedStanza, stanzaId);
        }

        protected async Task VerifySendAsyncMethod(Func<ISender<XmlElement>, XmlElement, Task<string>> asyncMethod, XmlElement expectedStanza)
        {
            var stanzaId = await asyncMethod.Invoke(sender, expectedStanza);

            AreEqual(expectedStanza, sendedStanza, stanzaId);
        }

        private void AreEqual(XmlElement expectedStanza, XmlElement sendedStanza, string stanzaId)
        {
            Assert.NotNull(sendedStanza);
            Assert.Equal(sendedStanza.Id(), stanzaId);
            sendedStanza.SetAttributeValue("id", null);
            Assert.Equal(sendedStanza, expectedStanza);
        }
    }
}
