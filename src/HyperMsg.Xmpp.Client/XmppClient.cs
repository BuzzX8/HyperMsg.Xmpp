using HyperMsg.Xmpp.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Xmpp.Client
{
    public class XmppClient : IXmppClient
    {        
        private readonly IMessageSender<XmlElement> sender;
        private readonly XmppConnectionSettings settings;

        private readonly IqStanzaHandler iqHandler;

        public XmppClient(IMessageSender<XmlElement> sender, XmppConnectionSettings settings)
        {            
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

            iqHandler = new IqStanzaHandler(sender);
        }        

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            //await publisher.PublishAsync(TransportMessage.Open, cancellationToken);
            //await publisher.PublishAsync(ReceiveMode.SetReactive, cancellationToken);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            await sender.SendEndOfStreamAsync(cancellationToken);
            //await publisher.PublishAsync(TransportMessage.Close, cancellationToken);
        }

        public Task<IEnumerable<RosterItem>> GetRosterAsync(CancellationToken cancellationToken) => iqHandler.SendRosterRequestAsync(settings.Jid, cancellationToken);

        public Task AddOrUpdateRosterItemAsync(RosterItem rosterItem, CancellationToken cancellationToken) => iqHandler.SendAddOrUpdateRosterItemAsync(rosterItem, cancellationToken);

        public Task RemoveRosterItemAsync(RosterItem rosterItem, CancellationToken cancellationToken) => iqHandler.SendItemRemoveRequestAsync(rosterItem, cancellationToken);

        public void Handle(XmlElement stanza)
        {            
            if (stanza.IsIq())
            {
                iqHandler.Handle(stanza);
            }
        }

        public Task HandleAsync(XmlElement stanza, CancellationToken cancellationToken)
        {
            Handle(stanza);
            return Task.CompletedTask;
        }
    }
}