using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Serialization
{
    public class ReadXmlTokenTests
    {
        [Theory]
        [InlineData("some-value", 0, XmlTokenType.None, "")]

        [InlineData("v<token", 1, XmlTokenType.Value, "v")]
        [InlineData("value<token", 5, XmlTokenType.Value, "value")]

        [InlineData("<?xml version='1.0'?>", 21, XmlTokenType.Declaration)]

        [InlineData("<token1>", 8, XmlTokenType.StartTag)]
        [InlineData("<   token-1 >", 13, XmlTokenType.StartTag)]
        [InlineData("<some-token attr1='val1' attr2='val2'>", 38, XmlTokenType.StartTag)]
        [InlineData("<some-token attr='some/attribute'>", 34, XmlTokenType.StartTag)]
        [InlineData("<token>value", 7, XmlTokenType.StartTag, "<token>")]

        [InlineData("<token3/>", 9, XmlTokenType.EnclosedTag)]
        [InlineData("< token-3  /             >", 26, XmlTokenType.EnclosedTag)]
        [InlineData("<tok attr2='val0' />", 20, XmlTokenType.EnclosedTag)]
        [InlineData("<tok attr2='val10' />  some-value", 21, XmlTokenType.EnclosedTag, "<tok attr2='val10' />")]

        [InlineData("</token2>", 9, XmlTokenType.ClosingTag)]
        [InlineData("<             /  token-2 >", 26, XmlTokenType.ClosingTag)]
        [InlineData("</token2> some-kind-value", 9, XmlTokenType.ClosingTag, "</token2>")]
        public static void ReadXmlToken_Returns_Correct_XmlToken(string xml, int expectedTokenSize, XmlTokenType tokenType, string expectedXml = null)
        {
            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            var expectedTokenBytes = xmlBytes;

            if (expectedXml != null)
            {
                expectedTokenBytes = Encoding.UTF8.GetBytes(expectedXml);
            }

            var buffer = new ReadOnlySequence<byte>(xmlBytes);
            var expectedToken = new XmlToken(new ReadOnlySequence<byte>(expectedTokenBytes), tokenType);

            (int actualTokenSize, var actualToken) = buffer.ReadXmlToken();

            Assert.Equal(expectedTokenSize, actualTokenSize);
            Assert.Equal(expectedToken.Type, actualToken.Type);
            Assert.Equal(expectedToken.BufferSegments.ToArray(), actualToken.BufferSegments.ToArray());
        }
                
        [Theory]        
        [InlineData("no-xml>-token")]
        [InlineData("no->xml<-token")]
        public static void ReadXmlToken_Throws_Exception_For_Invalid_Xml(string xml)
        {
            var buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(xml));
            Assert.Throws<DeserializationException>(() => buffer.ReadXmlToken());
        }
    }
}
