namespace HyperMsg.Xmpp.Client.Components
{
    internal static class StanzaExtensions
    {
        public static void ThrowIfStanzaError(this XmlElement element, string message)
        {
            if (element.IsStanza() && element.IsType("error"))
            {
                throw new XmppException(message);
            }
        }
    }
}
