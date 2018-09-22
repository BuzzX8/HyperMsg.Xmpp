using System;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Provides extension methods for fluent manipulation of stanza attributes.
    /// </summary>
    public static class StanzaExtensions
    {
        private static Func<string> IdGenerator = () =>
        {
            return DateTime.Now.Ticks.ToString("x");
        };

        public static string Id(this XmlElement element)
        {
            return element["id"];
        }

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
        public static XmlElement Id(this XmlElement element, string id)
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
        public static XmlElement NewId(this XmlElement element)
        {
            return element.Id(IdGenerator());
        }

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
        public static XmlElement From(this XmlElement element, string from)
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
        public static XmlElement To(this XmlElement element, string to)
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
        public static string Type(this XmlElement element)
        {
            return element.GetAttributeValue("type");
        }

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

        /// <summary>
        /// Returns true if value of 'type' attribute is 'get', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'get', otherwise false.
        /// </returns>
        public static bool IsGet(this XmlElement element)
        {
            return element.Type() == Iq.Type.Get;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'set', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'set', otherwise false.
        /// </returns>
        public static bool IsSet(this XmlElement element)
        {
            return element.Type() == Iq.Type.Set;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'result', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'result', otherwise false.
        /// </returns>
        public static bool IsResult(this XmlElement element)
        {
            return element.Type() == Iq.Type.Result;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'error', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'error', otherwise false.
        /// </returns>
        public static bool IsError(this XmlElement element)
        {
            return element.Type() == "error";
        }

        /// <summary>
        /// Returns true if no 'type' attribute, otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value no 'type' attribute, otherwise false.
        /// </returns>
        public static bool IsAvailable(this XmlElement presence)
        {
            return !presence.HasAttribute("type");
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'unavailable', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'unavailable', otherwise false.
        /// </returns>
        public static bool IsUnavailable(this XmlElement presence)
        {
            return presence.Type() == Presence.Type.Unavailable;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'subscribe', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'subscribe', otherwise false.
        /// </returns>
        public static bool IsSubscribe(this XmlElement presence)
        {
            return presence.Type() == Presence.Type.Subscribe;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'subscribed', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'subscribed', otherwise false.
        /// </returns>
        public static bool IsSubscribed(this XmlElement presence)
        {
            return presence.Type() == Presence.Type.Subscribed;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'unsubscribe', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'unsubscribe', otherwise false.
        /// </returns>
        public static bool IsUnsubscribe(this XmlElement presence)
        {
            return presence.Type() == Presence.Type.Unsubscribe;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'unsubscribed', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'unsubscribed', otherwise false.
        /// </returns>
        public static bool IsUnsubscribed(this XmlElement presence)
        {
            return presence.Type() == Presence.Type.Unsubscribed;
        }

        /// <summary>
        /// Returns true if value of 'type' attribute is 'probe', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if value of 'type' attribute is 'probe', otherwise false.
        /// </returns>
        public static bool IsProbe(this XmlElement presence)
        {
            return presence.Type() == Presence.Type.Probe;
        }

        /// <summary>
        /// Returns true if element name is 'iq', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'iq', otherwise false.
        /// </returns>
        public static bool IsIq(this XmlElement element)
        {
            return element.Name == "iq";
        }

        /// <summary>
        /// Returns true if element name is 'presence', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'presence', otherwise false.
        /// </returns>
        public static bool IsPresence(this XmlElement element)
        {
            return element.Name == "presence";
        }

        /// <summary>
        /// Returns true if element name is 'message', otherwise false.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <returns>
        /// true if element name is 'message', otherwise false.
        /// </returns>
        public static bool IsMessage(this XmlElement element)
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
            return element.IsIq()
                || element.IsPresence()
                || element.IsMessage();
        }

        /// <summary>
        /// Throws XmppException if <paramref name="element"/> is stanza and 'type' attribute
        /// is 'error'.
        /// </summary>
        /// <param name="element">
        /// XML element.
        /// </param>
        /// <param name="message">
        /// Exception message.
        /// </param>
        /// <exception cref="XmppException">
        /// Xml element is stanza and it 'type' is 'error'
        /// </exception>
        public static void ThrowIfStanzaError(this XmlElement element, string message)
        {
            if (element.IsStanza() && element.IsError())
            {
                throw new Exception(message);
            }
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
        public static bool IsEndOfStream(this XmlElement element)
        {
            return element.Name == "/stream:stream";
        }
    }
}