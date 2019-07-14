using HyperMsg.Xmpp.Client.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class XmppTransceivingExtensions
    {
        private static readonly XmlElement endOfStream = new XmlElement("/stream:stream");

        /// <summary>
        /// Asynchronously sends XML element that signals close XMPP stream.
        /// </summary>
        /// <param name="sender">
        ///  
        /// </param>
        /// <returns>
        /// Task that represent asynchronous send operation. It completes when closing XML element is send.
        /// </returns>
        public static async Task SendEndOfStreamAsync(this IMessageSender<XmlElement> sender, CancellationToken token = default) => await sender.SendAsync(endOfStream, token);

        /// <summary>
        /// Receives XML element from channel an verifies that it not stream error.
        /// </summary>
        /// <param name="receiver">
        /// XMPP channel.
        /// </param>
        /// <returns>
        /// Received XML element that is not stream error.
        /// </returns>
        /// <exception cref="XmppException">
        /// Received stream error (element with name stream:error).
        /// </exception>
        //public static XmlElement ReceiveNoStreamError(this IReceiver<XmlElement> receiver)
        //{
        //    var element = receiver.Receive();

        //    return ReturnIfNoStreamError(element);
        //}

        /// <summary>
        /// Asynchronously receives XML element from channel an verifies that it not stream error.
        /// </summary>
        /// <param name="receiver">
        /// XMPP channel.
        /// </param>
        /// <returns>
        /// Task that represents receive operation. The result of operation is XML element that is not stream error.
        /// </returns>
        /// <exception cref="XmppException">
        /// Received stream error (element with name stream:error).
        /// </exception>
        //public static async Task<XmlElement> ReceiveNoStreamErrorAsync(this IReceiver<XmlElement> receiver, CancellationToken token = default)
        //{
        //    var element = await receiver.ReceiveAsync(token);

        //    return ReturnIfNoStreamError(element);
        //}

        private static XmlElement ReturnIfNoStreamError(XmlElement element)
        {
            if (element.Name == "stream:error")
            {
                throw new XmppException(Resources.StreamErrorReceived, element);
            }

            return element;
        }
    }
}
