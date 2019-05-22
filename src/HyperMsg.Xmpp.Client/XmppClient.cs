using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.StreamNegotiation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class XmppClient : IXmppClient
    {
        private readonly IStreamNegotiator streamNegotiator;
        private readonly ITransceiver<XmlElement, XmlElement> transceiver;
        private readonly XmppConnectionSettings settings;

        private readonly IHandler<TransportCommands> transportHandler;
        private readonly IHandler<ReceiveMode> receiveModeHandler;

        public XmppClient(IStreamNegotiator streamNegotiator, ITransceiver<XmlElement, XmlElement> transceiver, XmppConnectionSettings settings, 
            IHandler<TransportCommands> transportHandler, IHandler<ReceiveMode> receiveModeHandler)
        {
            this.streamNegotiator = streamNegotiator ?? throw new ArgumentNullException(nameof(streamNegotiator));
            this.transceiver = transceiver ?? throw new ArgumentNullException(nameof(transceiver));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.transportHandler = transportHandler ?? throw new ArgumentNullException(nameof(transportHandler));
            this.receiveModeHandler = receiveModeHandler ?? throw new ArgumentNullException(nameof(receiveModeHandler));
        }

        public void Connect() => ConnectAsync().GetAwaiter().GetResult();

        public async Task ConnectAsync(CancellationToken token = default)
        {
            await transportHandler.HandleAsync(TransportCommands.OpenConnection, token);
            await streamNegotiator.NegotiateAsync(transceiver, settings, token);
            await receiveModeHandler.HandleAsync(ReceiveMode.Reactive);
        }

        public void Disconnect() => DisconnectAsync().GetAwaiter().GetResult();

        public async Task DisconnectAsync(CancellationToken token = default)
        {
            await receiveModeHandler.HandleAsync(ReceiveMode.Proactive, token);
            await transceiver.SendEndOfStreamAsync(token);
            await transportHandler.HandleAsync(TransportCommands.CloseConnection);
        }
    }
}
