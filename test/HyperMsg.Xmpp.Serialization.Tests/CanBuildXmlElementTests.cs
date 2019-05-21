using Xunit;

namespace HyperMsg.Xmpp.Serialization
{
    public class CanBuildXmlElementTests
    {
        [Theory]
        [InlineData("<element>", false)]
        [InlineData("<element>value", false)]
        [InlineData("<element></element>", true)]
        [InlineData("<element>value</element>", true)]
        [InlineData("<element/>", true)]
        public void CanBuildXmlElement_Returns_Value_Indicating_That_Xml_Tokens_Can_Be_Translated_Into_Elements(string xml, bool expectedResult)
        {
            var tokens = xml.GetTokens();

            var actualResult = tokens.CanBuildXmlElement();

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
