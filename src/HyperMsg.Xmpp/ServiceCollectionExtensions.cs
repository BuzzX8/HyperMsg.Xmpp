using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Xmpp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXmppServices(this IServiceCollection services) =>
            services.AddHostedService<BufferSerializationService>()
                .AddHostedService<StreamNegotiationService>()
                .AddHostedService<TlsNegotiator>()
                .AddHostedService<BindNegotiator>()
                .AddHostedService<SessionNegotiator>()
                .AddHostedService<RosterService>()
                .AddHostedService<PresenceService>()
                .AddHostedService<MessageService>();
    }
}
