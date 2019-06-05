using HyperMsg.Xmpp.Client.Extensions;
using HyperMsg.Xmpp.Client.StreamNegotiation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class XmppClient : IXmppClient, IHandler<XmlElement>
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

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await transportHandler.HandleAsync(TransportCommands.OpenConnection, cancellationToken);
            await streamNegotiator.NegotiateAsync(transceiver, settings, cancellationToken);
            await receiveModeHandler.HandleAsync(ReceiveMode.Reactive, cancellationToken);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            await transceiver.SendEndOfStreamAsync(cancellationToken);
            await transportHandler.HandleAsync(TransportCommands.CloseConnection, cancellationToken);
        }

        public async Task<IEnumerable<RosterItem>> GetRosterAsync(CancellationToken cancellationToken)
        {
            await transceiver.SendAsync(Roster.Get(), cancellationToken);

            throw new NotImplementedException();
        }

        public Task AddOrUpdateRosterItem(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRosterItem(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Handle(XmlElement stanza)
        {            
            if (stanza.IsIq())
            {

            }
        }

        public Task HandleAsync(XmlElement stanza, CancellationToken cancellationToken)
        {
            Handle(stanza);
            return Task.CompletedTask;
        }
    }
}