using HyperMsg.Extensions;
using HyperMsg.Xmpp.Serialization;
using HyperMsg.Xmpp.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Xmpp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXmppSerialization(this IServiceCollection services)
        {
            return services.AddSerializationComponent<XmlElement>(XmlSerializer.Serialize)
                .AddDeserializationComponent(XmlDeserializer.Deserialize);
        }
    }
}
