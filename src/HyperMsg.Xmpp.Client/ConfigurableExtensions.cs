namespace HyperMsg.Xmpp.Client
{
    public static class ConfigurableExtensions
    {
        public static void UseRosterService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IRosterService), (p, s) =>
            {
                var sender = (IMessageSender<XmlElement>)p.GetService(typeof(IMessageSender<XmlElement>));
                var registry = (IMessageHandlerRegistry<XmlElement>)p.GetService(typeof(IMessageHandlerRegistry<XmlElement>));

                var service = new RosterService(sender);
                registry.Register(service.Handle);

                return service;
            });
        }

        public static void UsePresenceService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IPresenceService), (p, s) =>
            {
                var sender = (IMessageSender<XmlElement>)p.GetService(typeof(IMessageSender<XmlElement>));
                var registry = (IMessageHandlerRegistry<XmlElement>)p.GetService(typeof(IMessageHandlerRegistry<XmlElement>));

                var service = new PresenceService(sender);
                registry.Register(service.Handle);

                return service;
            });
        }

        public static void UsePresenceSubscriptionService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IPresenceSubscriptionService), (p, s) =>
            {
                var sender = (IMessageSender<XmlElement>)p.GetService(typeof(IMessageSender<XmlElement>));
                var registry = (IMessageHandlerRegistry<XmlElement>)p.GetService(typeof(IMessageHandlerRegistry<XmlElement>));

                var service = new PresenceSubscriptionService(sender);
                registry.Register(service.Handle);

                return service;
            });
        }
        
        public static void UseMessageService(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IMessageService), (p, s) =>
            {
                var sender = (IMessageSender<XmlElement>)p.GetService(typeof(IMessageSender<XmlElement>));
                var registry = (IMessageHandlerRegistry<XmlElement>)p.GetService(typeof(IMessageHandlerRegistry<XmlElement>));

                var service = new MessageService(sender);
                registry.Register(service.Handle);

                return service;
            });
        }
    }
}
