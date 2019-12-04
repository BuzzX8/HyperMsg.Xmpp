using HyperMsg.Xmpp.Client.Components;

namespace HyperMsg.Xmpp.Client
{
    public static class ConfigurableExtensions
    {
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
