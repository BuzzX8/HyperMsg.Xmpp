namespace HyperMsg.Xmpp
{
    public class StreamFooter : XmlElement
    {
        private StreamFooter() : base("stream:stream")
        { }

        public static StreamFooter Instance { get; } = new StreamFooter();
    }
}
