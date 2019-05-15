using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Serialization
{
    public class ReadXmlTokenTests
    {
        public static IEnumerable<object[]> GetTestCases()
        {            
            yield return GetTestCase("<token1>", 8, XmlTokenType.StartTag, "token1");
            yield return GetTestCase("<   token-1 >", 13, XmlTokenType.StartTag, "token-1");
            yield return GetTestCase("</token2>", 9, XmlTokenType.ClosingTag, "token2");
            yield return GetTestCase("<             /  token-2 >", 26, XmlTokenType.ClosingTag, "token-2");
            yield return GetTestCase("<token3/>", 9, XmlTokenType.EnclosedTag, "token3");
            yield return GetTestCase("< token-3  /             >", 26, XmlTokenType.EnclosedTag, "token-3");
            yield return GetTestCase("<token1>value</token1>", 8, XmlTokenType.StartTag, "token1");
            yield return GetTestCase("<some-token attr1='val1' attr2='val2'>", 38, XmlTokenType.StartTag, "some-token");
            yield return GetTestCase("<some-token attr='some/attribute'>", 34, XmlTokenType.StartTag, "some-token");
            yield return GetTestCase("<tok attr2='val0' />", 20, XmlTokenType.EnclosedTag, "tok");
            yield return GetTestCase("<?xml version='1.0'?>", 21, XmlTokenType.Declaration, string.Empty);
        }

        public static object[] GetTestCase(string xml, int tokenSize, XmlTokenType tokenType, string tokenName)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            return new object[] { new ReadOnlySequence<byte>(bytes), tokenSize, new XmlToken(tokenType, tokenName) };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public static void ReadXmlToken_Returns_Correct_XmlToken(ReadOnlySequence<byte> buffer, int expectedTokenSize, XmlToken expectedToken)
        {
            (int actualTokenSize, var actualToken) = buffer.ReadXmlToken();

            Assert.Equal(expectedTokenSize, actualTokenSize);
            Assert.Equal(expectedToken, actualToken);
        }
                
        [Theory]
        [InlineData("")]
        [InlineData("no-xml-tokens")]
        [InlineData("no-xml-<token")]
        [InlineData("no-xml>-token")]
        [InlineData("no->xml<-token")]
        public static void ReadXmlToken_Throws_Exception_For_Invalid_Xml(string xml)
        {
            var buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(xml));
            Assert.Throws<DeserializationException>(() => buffer.ReadXmlToken());
        }
    }
}
