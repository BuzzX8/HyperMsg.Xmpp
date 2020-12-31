using HyperMsg.Xmpp.Extensions;

namespace HyperMsg.Xmpp.Xml
{
    /// <summary>
    /// Provides static methods for creating client and server stream headers.
    /// </summary>
    public static class StreamHeader
    {
        /// <summary>
        /// Creates client header.
        /// </summary>
        /// <returns>
        /// Element that represents client stream header.
        /// </returns>
        public static XmlElement Client()
        {
            return CreateBasicHeader()
                .Xmlns(XmppNamespaces.JabberClient);
        }

        /// <summary>
        /// Creates server header.
        /// </summary>
        /// <returns>
        /// Element that represents server stream header.
        /// </returns>
        public static XmlElement Server()
        {
            return CreateBasicHeader()
                .Xmlns(XmppNamespaces.JabberServer);
        }

        private static XmlElement CreateBasicHeader()
        {
            return new XmlElement("stream:stream")
                .Attribute("xmlns:stream", XmppNamespaces.Streams)
                .Attribute("version", "1.0");
        }
    }
}
