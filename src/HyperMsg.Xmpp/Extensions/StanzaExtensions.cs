using HyperMsg.Xmpp.Xml;
using System;

namespace HyperMsg.Xmpp.Extensions
{
    /// <summary>
    /// Provides extension methods for fluent manipulation of stanza attributes.
    /// </summary>
    public static class StanzaExtensions
    {
        private static readonly Func<string> IdGenerator = () => DateTime.Now.Ticks.ToString("x");

        public static string Id(this XmlElement element) => element["id"];

        /// <summary>
        /// Updates value of 'id' attribute.
        /// </summary>
        /// <param name="element">
        /// Element which attribute should be updated.
        /// </param>
        /// <param name="id">
        /// Value of id attribute.
        /// </param>
        /// <returns>
        /// Element with updated value of 'id' attribute.
        /// </returns>
        public static XmlElement Id(this XmlElement element, object id)
        {
            element.SetAttributeValue("id", id);

            return element;
        }

        /// <summary>
        /// Updates value of 'id' attribute with random value.
        /// </summary>
        /// <param name="element">
        /// Element which attribute should be updated.
        /// </param>
        /// <returns>
        /// Element with updated 'id' attribute.
        /// </returns>
        public static XmlElement NewId(this XmlElement element) => element.Id(IdGenerator.Invoke());

        /// <summary>
        /// Updates value of 'from' attributes.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be updated.
        /// </param>
        /// <param name="from">
        /// Value of 'from' attribute.
        /// </param>
        /// <returns>
        /// Element with updated 'from' attribute.
        /// </returns>
        public static XmlElement From(this XmlElement element, Jid from)
        {
            element.SetAttributeValue("from", from);

            return element;
        }

        /// <summary>
        /// Updates value of 'to' attributes.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be updated.
        /// </param>
        /// <param name="to">
        /// Value of 'to' attribute.
        /// </param>
        /// <returns>
        /// Element with updated 'to' attribute.
        /// </returns>
        public static XmlElement To(this XmlElement element, Jid to)
        {
            element.SetAttributeValue("to", to);

            return element;
        }

        /// <summary>
        /// Returns value of 'type' attribute.
        /// </summary>
        /// <param name="element">
        /// Element which value should be readed.
        /// </param>
        /// <returns>
        /// Value of 'type' attribute.
        /// </returns>
        public static string Type(this XmlElement element) => element.GetAttributeValue("type");

        /// <summary>
        /// Updates value of 'type' attributes.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be updated.
        /// </param>
        /// <param name="type">
        /// Value of 'type' attribute.
        /// </param>
        /// <returns>
        /// Element with updated 'type' attribute.
        /// </returns>
        public static XmlElement Type(this XmlElement element, string type)
        {
            element.SetAttributeValue("type", type);

            return element;
        }

        public static bool IsType(this XmlElement element, string type) => element.Type() == type;

        /// <summary>
        /// Returns true if element name is 'iq', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'iq', otherwise false.
        /// </returns>
        public static bool IsIqStanza(this XmlElement element) => element.Name == "iq";

        /// <summary>
        /// Returns true if element name is 'presence', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'presence', otherwise false.
        /// </returns>
        public static bool IsPresenceStanza(this XmlElement element) => element.Name == "presence";

        /// <summary>
        /// Returns true if element name is 'message', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'message', otherwise false.
        /// </returns>
        public static bool IsMessageStanza(this XmlElement element)
        {
            return element.Name == "message";
        }

        /// <summary>
        /// Returns true if element name is 'iq', 'presence' or 'message'. Otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'iq', 'presence' or 'message'. Otherwise false.
        /// </returns>
        public static bool IsStanza(this XmlElement element)
        {
            return element.IsIqStanza()
                || element.IsPresenceStanza()
                || element.IsMessageStanza();
        }

        /// <summary>
        /// Returns true if XML element is '</stream:stream>' which is stream closing tag (RFC 6120 p4.4)
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element is closing tag with name 'stream:stream'
        /// </returns>
        public static bool IsEndOfStream(this XmlElement element) => element.Name == "/stream:stream";

        public static void ThrowIfStanzaError(this XmlElement element, string message)
        {
            if (element.IsStanza() && element.IsType("error"))
            {
                throw new XmppException(message);
            }
        }
    }
}