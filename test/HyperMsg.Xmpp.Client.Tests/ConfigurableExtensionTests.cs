using FakeItEasy;
using Xunit;

namespace HyperMsg.Xmpp.Client
{
    public class ConfigurableExtensionTests
    {
        private readonly IConfigurable configurable;

        public ConfigurableExtensionTests()
        {
            configurable = A.Fake<IConfigurable>();
        }

        [Fact]
        public void UseRosterService_Register_Factory_For_RosterService()
        {
            configurable.UseRosterService();

            A.CallTo(() => configurable.RegisterService(typeof(IRosterService), A<ServiceFactory>._)).MustHaveHappened();
        }

        [Fact]
        public void UsePresenceService_Register_Factory_For_PresenceService()
        {
            configurable.UsePresenceService();

            A.CallTo(() => configurable.RegisterService(typeof(IPresenceService), A<ServiceFactory>._)).MustHaveHappened();
        }

        [Fact]
        public void UsePresenceSubscriptionService_Register_PresenceSubscriptionService()
        {
            configurable.UsePresenceSubscriptionService();

            A.CallTo(() => configurable.RegisterService(typeof(IPresenceSubscriptionService), A<ServiceFactory>._)).MustHaveHappened();
        }

        [Fact]
        public void UseMessageService_Register_PresenceSubscriptionService()
        {
            configurable.UseMessageService();

            A.CallTo(() => configurable.RegisterService(typeof(IMessageService), A<ServiceFactory>._)).MustHaveHappened();
        }
    }
}
