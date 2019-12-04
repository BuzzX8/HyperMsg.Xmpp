using HyperMsg.Xmpp.Client.Components;

namespace HyperMsg.Xmpp.Client
{
    public static class ConfigurableExtensions
    {
        public static void UseXmppServices(this IConfigurable configurable, XmppConnectionSettings connectionSettings)
        {
            configurable.UseXmppConnection(connectionSettings);
            configurable.UseRosterService();
            configurable.UsePresenceSubscriptionService();
            configurable.UsePresenceService();
            configurable.UseMessageService();
        }

        public static void UseXmppConnection(this IConfigurable configurable, XmppConnectionSettings connectionSettings)
        {
            string ConnectionSettingName = connectionSettings.GetType().FullName;

            configurable.AddSetting(ConnectionSettingName, connectionSettings);
            configurable.RegisterConfigurator((p, s) =>
            {
                var messageSender = (IMessageSender)p.GetService(typeof(IMessageSender));
                var handlerRegistry = (IMessageHandlerRegistry)p.GetService(typeof(IMessageHandlerRegistry));
                var component = new ConnectionComponent(messageSender, (XmppConnectionSettings)s[ConnectionSettingName]);
                handlerRegistry.Register<TransportEvent>(component.HandleTransportEventAsync);
                handlerRegistry.Register<Received<XmlElement>>(component.HandleAsync);
            });
        }

        public static void UseRosterService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IRosterService), (p, s) =>
            {
                var sender = (IMessageSender)p.GetService(typeof(IMessageSender));
                var registry = (IMessageHandlerRegistry)p.GetService(typeof(IMessageHandlerRegistry));

                var service = new RosterComponent(sender);
                registry.Register<XmlElement>(service.Handle);

                return service;
            });
        }

        public static void UsePresenceService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IPresenceService), (p, s) =>
            {
                var sender = (IMessageSender)p.GetService(typeof(IMessageSender));
                var registry = (IMessageHandlerRegistry)p.GetService(typeof(IMessageHandlerRegistry));

                var service = new PresenceComponent(sender);
                registry.Register<XmlElement>(service.Handle);

                return service;
            });
        }

        public static void UsePresenceSubscriptionService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IPresenceSubscriptionService), (p, s) =>
            {
                var sender = (IMessageSender)p.GetService(typeof(IMessageSender));
                var registry = (IMessageHandlerRegistry)p.GetService(typeof(IMessageHandlerRegistry));

                var service = new PresenceSubscriptionComponent(sender);
                registry.Register<XmlElement>(service.Handle);

                return service;
            });
        }
        
        public static void UseMessageService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IMessageService), (p, s) =>
            {
                var sender = (IMessageSender)p.GetService(typeof(IMessageSender));
                var registry = (IMessageHandlerRegistry)p.GetService(typeof(IMessageHandlerRegistry));

                var service = new MessagingComponent(sender);
                registry.Register<XmlElement>(service.Handle);

                return service;
            });
        }
    }
}
