using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client.Extensions
{
    public static class StanzaExtensions
    {
        public static string SendWithNewId(this ISender<XmlElement> channel, XmlElement stanza)
        {
            stanza = stanza.NewId();
            channel.Send(stanza);
            return stanza.Id();
        }

        public static async Task<string> SendWithNewIdAsync(this ISender<XmlElement> channel, XmlElement stanza, CancellationToken token = default)
        {
            stanza = stanza.NewId();
            await channel.SendAsync(stanza, token);
            return stanza.Id();
        }
    }
}
