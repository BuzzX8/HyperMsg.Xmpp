using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Xmpp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXmppServices(this IServiceCollection services) =>
            services.AddHostedService<StreamNegotiationService>()
                .AddHostedService<TlsNegotiator>()
                .AddHostedService<BindNegotiator>()
                .AddHostedService<SessionNegotiator>();
    }
}
