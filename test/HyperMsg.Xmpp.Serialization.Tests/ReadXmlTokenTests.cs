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
            yield return GetTestCase("some-value", 0, XmlTokenType.None, string.Empty);
            yield return GetTestCase("<?xml version='1.0'?>", 21, XmlTokenType.Declaration);

            yield return GetTestCase("<token1>", 8, XmlTokenType.StartTag);
            yield return GetTestCase("<   token-1 >", 13, XmlTokenType.StartTag);
            yield return GetTestCase("<some-token attr1='val1' attr2='val2'>", 38, XmlTokenType.StartTag);
            yield return GetTestCase("<some-token attr='some/attribute'>", 34, XmlTokenType.StartTag);
            yield return GetTestCase("<token>value", 7, XmlTokenType.StartTag, "<token>");

            yield return GetTestCase("<token3/>", 9, XmlTokenType.EnclosedTag);
            yield return GetTestCase("< token-3  /             >", 26, XmlTokenType.EnclosedTag);
            yield return GetTestCase("<tok attr2='val0' />", 20, XmlTokenType.EnclosedTag);
            yield return GetTestCase("<tok attr2='val10' />  some-value", 21, XmlTokenType.EnclosedTag, "<tok attr2='val10' />");

            yield return GetTestCase("</token2>", 9, XmlTokenType.ClosingTag);
            yield return GetTestCase("<             /  token-2 >", 26, XmlTokenType.ClosingTag);
            yield return GetTestCase("</token2> some-kind-value", 9, XmlTokenType.ClosingTag, "</token2>");
        }

        public static object[] GetTestCase(string xml, int tokenSize, XmlTokenType tokenType, string expectedToken = null)
        {
            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            var expectedTokenBytes = xmlBytes;

            if (expectedToken != null)
            {
                expectedTokenBytes = Encoding.UTF8.GetBytes(expectedToken);
            }

            return new object[] { new ReadOnlySequence<byte>(xmlBytes), tokenSize, new XmlToken(new ReadOnlySequence<byte>(expectedTokenBytes), tokenType) };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public static void ReadXmlToken_Returns_Correct_XmlToken(ReadOnlySequence<byte> buffer, int expectedTokenSize, XmlToken expectedToken)
        {
            (int actualTokenSize, var actualToken) = buffer.ReadXmlToken();

            Assert.Equal(expectedTokenSize, actualTokenSize);
            Assert.Equal(expectedToken.Type, actualToken.Type);
            Assert.Equal(expectedToken.BufferSegments.ToArray(), actualToken.BufferSegments.ToArray());
        }
                
        [Theory]
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
