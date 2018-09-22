using Xunit;

namespace HyperMsg.Xmpp.Tests
{
    public class IqTests
    {
        [Fact]
        public void Get_Creates_Iq_Stanza_With_Type_Get()
        {
            var stanza = Iq.Get();

            Assert.Equal(Iq.Type.Get, stanza.Type());
        }

        [Fact]
        public void Set_Creates_Iq_Stanza_With_Set_Type()
        {
            var stanza = Iq.Set();

            Assert.Equal(Iq.Type.Set, stanza.Type());
        }

        [Fact]
        public void Result_Creates_Iq_Stanza_With_Result_Type()
        {
            var stanza = Iq.Result();

            Assert.Equal(Iq.Type.Result, stanza.Type());
        }

        [Fact]
        public void Error_Creates_Iq_Stanza_With_Error_Type()
        {
            var stanza = Iq.Error();

            Assert.Equal(Iq.Type.Error, stanza.Type());
        }
    }
}
