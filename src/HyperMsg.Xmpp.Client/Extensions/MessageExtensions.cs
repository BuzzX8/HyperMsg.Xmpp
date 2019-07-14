using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class MessageExtensions
    {
        public static Task<string> SendMessageAsync(this IMessageSender<XmlElement> channel, Jid to, MessageType type, string text)
        {
            var messageStanza = CreateMessageStanza(to, type, string.Empty, text);

            return channel.SendWithNewIdAsync(messageStanza);
        }

        public static Task<string> SendMessageAsync(this IMessageSender<XmlElement> channel, Jid to, MessageType type, string subject, string text)
        {
            var messageStanza = CreateMessageStanza(to, type, subject, text);

            return channel.SendWithNewIdAsync(messageStanza);
        }

        private static XmlElement CreateMessageStanza(Jid to, MessageType type, string subject, string text)
        {
            var message = new XmlElement("message").To(to);
            SetMessageType(message, type);
            SetSubjectAndBody(message, subject, text);
            return message;
        }

        private static void SetMessageType(XmlElement message, MessageType type)
        {
            message.SetAttributeValue("type", type.ToString().ToLower());
        }

        private static void SetSubjectAndBody(XmlElement message, string subject, string body)
        {
            if(!string.IsNullOrEmpty(subject))
            {
                message.Children.Add(new XmlElement("subject").Value(subject));
            }

            message.Children.Add(new XmlElement("body").Value(body));
        }
    }
}
