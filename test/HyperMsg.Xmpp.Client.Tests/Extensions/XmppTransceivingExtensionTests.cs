using FakeItEasy;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public class XmppTransceivingExtensionTests
    {
        //private readonly ITransceiver<XmlElement, XmlElement> transceiver;
        //private XmlElement sentElement = null;

        //public XmppTransceivingExtensionTests()
        //{
        //    transceiver = A.Fake<ITransceiver<XmlElement, XmlElement>>();
        //    A.CallTo(() => transceiver.Send(A<XmlElement>._))
        //        .Invokes(foc => sentElement = foc.GetArgument<XmlElement>(0));
        //    A.CallTo(() => transceiver.SendAsync(A<XmlElement>._, A<CancellationToken>._))
        //        .Invokes(foc => sentElement = foc.GetArgument<XmlElement>(0))
        //        .Returns(Task.CompletedTask);
        //}

        //[Fact]
        //public void SendEndOfStream_Sends_End_Of_Stream_Element()
        //{
        //    transceiver.SendEndOfStream();            

        //    Assert.True(sentElement.IsEndOfStream());
        //}

        //[Fact]
        //public async Task SendEndOfStreamAsync_Sends_End_Of_Stream_Element()
        //{
        //    await transceiver.SendEndOfStreamAsync();
            
        //    Assert.Equal(@"/stream:stream", sentElement.Name);
        //}

        //[Fact]
        //public void ReceiveNoStreamError_Returns_Received_Element_If_It_No_Stream_Error()
        //{
        //    var expected = new XmlElement("stream:features");
        //    A.CallTo(() => transceiver.Receive()).Returns(expected);

        //    var actual = transceiver.ReceiveNoStreamError();

        //    Assert.Equal(expected, actual);
        //}

        //[Fact]
        //public async Task ReceiveNoStreamErrorAsync_Returns_Received_Element_If_It_No_Stream_Error()
        //{
        //    var expected = new XmlElement("stream:features");
        //    A.CallTo(() => transceiver.ReceiveAsync(A<CancellationToken>._)).Returns(Task.FromResult(expected));

        //    var actual = await transceiver.ReceiveNoStreamErrorAsync();

        //    Assert.Equal(expected, actual);
        //}

        //[Fact]
        //public void ReceiveNoStreamError_Throws_Exception_If_Stream_Error_Received()
        //{
        //    var error = new XmlElement("stream:error");
        //    A.CallTo(() => transceiver.Receive()).Returns(error);

        //    Assert.Throws<XmppException>(() => transceiver.ReceiveNoStreamError());
        //}

        //[Fact]
        //public void ReceiveNoStreamErrorAsync_Throws_Exception_If_Stream_Error_Received()
        //{
        //    var error = new XmlElement("stream:error");
        //    A.CallTo(() => transceiver.ReceiveAsync(A<CancellationToken>._)).Returns(Task.FromResult(error));

        //    Assert.ThrowsAsync<XmppException>(() => transceiver.ReceiveNoStreamErrorAsync());
        //}
    }
}
