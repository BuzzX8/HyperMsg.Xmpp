using FakeItEasy;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public class StanzaExtensionTests
    {
        [Fact]
        public async Task SendWithNewIdAsync_Generates_New_Id_And_Sends_Element_To_Channel()
        {
            IMessageSender<XmlElement> channel = A.Fake<IMessageSender<XmlElement>>();
            XmlElement sendedElement = null;
            A.CallTo(() => channel.SendAsync(A<XmlElement>._, A<CancellationToken>._)).Invokes(foc =>
            {
                sendedElement = foc.GetArgument<XmlElement>(0);
            });
            XmlElement element = new XmlElement("some-element");

            string id = await channel.SendWithNewIdAsync(element);

            Assert.NotNull(sendedElement);
            Assert.Equal(id, sendedElement.Id());
            Assert.Equal(element.Name, sendedElement.Name);
        }

    }
}
