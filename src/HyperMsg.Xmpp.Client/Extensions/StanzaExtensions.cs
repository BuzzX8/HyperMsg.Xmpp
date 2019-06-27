using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class StanzaExtensions
    {
        public static async Task<string> SendWithNewIdAsync(this IMessageSender<XmlElement> channel, XmlElement stanza, CancellationToken token = default)
        {
            stanza = stanza.NewId();
            await channel.SendAsync(stanza, token);
            return stanza.Id();
        }
    }
}
