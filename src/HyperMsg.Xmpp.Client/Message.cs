namespace HyperMsg.Xmpp.Client
{
    public class Message
    {
        public MessageType Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public override bool Equals(object obj)
        {
            var msg = obj as Message;

            if (msg == null)
            {
                return false;
            }

            return Type.Equals(msg.Type)
                && string.Equals(Subject, msg.Subject)
                && string.Equals(Body, msg.Body);

        }
    }
}