using HyperMsg.Xmpp.Xml;
using System;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// The exception that is thrown when certain XMPP protocol errors occurs (like stream or stanza error).
    /// </summary>
    public class XmppException : Exception
    {
        /// <summary>
        /// Initializes new instance of XmppException.
        /// </summary>
        public XmppException()
        { }

        /// <summary>
        /// Initializes new instance of XmppException.
        /// </summary>
        /// <param name="message">
        /// A value that represents error message.
        /// </param>
        public XmppException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes new instance of XmppException.
        /// </summary>
        /// <param name="message">
        /// A value that represents error message.
        /// </param>
        /// <param name="inner">
        /// The value of InnerException property, which represents the cause of current problem.
        /// </param>
        public XmppException(string message, Exception inner) : base(message, inner)
        { }

        /// <summary>
        /// Initializes new instance of XmppException.
        /// </summary>
        /// <param name="errorElement">
        /// Xml element that represents XMPP error.
        /// </param>
        public XmppException(XmlElement errorElement)
        {
            ErrorElement = errorElement;
        }

        /// <summary>
        /// Initializes new instance of XmppException.
        /// </summary>
        /// <param name="message">
        /// A value that represents error message.
        /// </param>
        /// <param name="errorElement">
        /// Xml element that represents XMPP error.
        /// </param>
        public XmppException(string message, XmlElement errorElement) : base(message)
        {
            ErrorElement = errorElement;
        }

        /// <summary>
        /// Returns Xml element that represents XMPP error.
        /// </summary>
        public XmlElement ErrorElement { get; private set; }
    }
}