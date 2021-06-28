namespace HyperMsg.Xmpp.Xml
{
    public class StreamFooter : XmlElement
    {
        private StreamFooter() : base("stream:stream")
        { }

        public static StreamFooter Instance { get; } = new StreamFooter();
    }
}
