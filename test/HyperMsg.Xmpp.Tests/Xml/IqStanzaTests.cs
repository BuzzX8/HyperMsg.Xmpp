using Xunit;

namespace HyperMsg.Xmpp.Xml
{
    public class IqStanzaTests
    {
        [Fact]
        public void New_Creates_XmlElement_With_Name_Iq()
        {
            var stanza = IqStanza.New();

            Assert.Equal("iq", stanza.Name);
        }

        [Fact]
        public void Get_Creates_Iq_Stanza_With_Type_Get()
        {
            AssertIqStanza(IqStanza.Get(), IqStanza.Type.Get);
        }

        [Fact]
        public void Set_Creates_Iq_Stanza_With_Set_Type()
        {
            AssertIqStanza(IqStanza.Set(), IqStanza.Type.Set);
        }

        [Fact]
        public void Result_Creates_Iq_Stanza_With_Result_Type()
        {
            AssertIqStanza(IqStanza.Result(), IqStanza.Type.Result);
        }

        [Fact]
        public void Error_Creates_Iq_Stanza_With_Error_Type()
        {
            AssertIqStanza(IqStanza.Error(), IqStanza.Type.Error);
        }

        private void AssertIqStanza(XmlElement stanza, string type)
        {
            Assert.Equal("iq", stanza.Name);
            Assert.Equal(type, stanza.Type());
        }
    }
}
