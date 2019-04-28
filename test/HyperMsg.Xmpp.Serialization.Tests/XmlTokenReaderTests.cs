using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Serialization
{

    public class XmlTokenReaderTests
    {
        private ArraySegment<byte> buffer;
        private XmlTokenReader reader;

        public XmlTokenReaderTests()
        {
            buffer = new ArraySegment<byte>(new byte[512], 0, 512);
            reader = new XmlTokenReader();
        }

        public static IEnumerable<object[]> HasTokensTestCases()
        {
            yield return new object[] { "", false };
            yield return new object[] { "no-xml-tokens", false };
            yield return new object[] { "no-xml-<token", false };
            yield return new object[] { "no-xml>-token", false };
            yield return new object[] { "no->xml<-token", false };
            yield return new object[] { "<token>", true };
            yield return new object[] { "<token attr='some/attribute'>", true };
        }

        [Theory]
        [MemberData(nameof(HasTokensTestCases))]
        public void HasTokens_Returns_True_If_Theres_Xml_Tokens(string xml, bool expected)
        {
            WriteToBuffer(xml);

            Assert.Equal(reader.HasTokens(buffer), expected);
        }

        public static IEnumerable<object[]> HasTokensAfterReadTestCases()
        {
            yield return new object[] { "<token>", false };
            yield return new object[] { "<token1><token2>", true };
            yield return new object[] { "<token1>some-value<token2>", true };
        }

        [Theory]
        [MemberData(nameof(HasTokensAfterReadTestCases))]
        public void HasTokens_Returns_Correct_Value_After_Read(string xml, bool expectedValue)
        {
            WriteToBuffer(xml);

            Assert.True(reader.Read(buffer));

            Assert.Equal(reader.HasTokens(buffer), expectedValue);
        }

        public static IEnumerable<object[]> ReadTestCases()
        {
            yield return new object[] { "not-a-<token", null, XmlTokenType.None, false };
            yield return new object[] { "<token1>", "token1", XmlTokenType.StartTag, true };
            yield return new object[] { "<   token-1 >", "token-1", XmlTokenType.StartTag, true };
            yield return new object[] { "</token2>", "token2", XmlTokenType.ClosingTag, true };
            yield return new object[] { "<             /  token-2 >", "token-2", XmlTokenType.ClosingTag, true };
            yield return new object[] { "<token3/>", "token3", XmlTokenType.EnclosedTag, true };
            yield return new object[] { "< token-3  /             >", "token-3", XmlTokenType.EnclosedTag, true };
            yield return new object[] { "<token1>value</token1>", "token1", XmlTokenType.StartTag, true };
            yield return new object[] { "<some-token attr1='val1' attr2='val2'>", "some-token", XmlTokenType.StartTag, true };
            yield return new object[] { "<some-token attr='some/attribute'>", "some-token", XmlTokenType.StartTag, true };
            yield return new object[] { "<tok attr2='val0' />", "tok", XmlTokenType.EnclosedTag, true };
            yield return new object[] { "<?xml version='1.0'?>", string.Empty, XmlTokenType.Declaration, true };
        }

        [MemberData(nameof(ReadTestCases))]
        [Theory(DisplayName = "Read updates TokenName and TokenType properties")]
        public void Read_Correctly_Updates_TokenName_And_TokenType(string xml, string tokenName, XmlTokenType tokenType, bool readResult)
        {
            WriteToBuffer(xml);

            Assert.Equal(reader.Read(buffer), readResult);
            VerifyReader(tokenName, tokenType);
        }

        public static IEnumerable<object[]> ReadAftrerAdvancementTestCases()
        {
            yield return new object[] { "</token2>", null, XmlTokenType.None, false };
            yield return new object[] { "<token1><token2/>", "token2", XmlTokenType.EnclosedTag, true };
            yield return new object[] { "<token1>     </ token2>", "token2", XmlTokenType.ClosingTag, true };
            yield return new object[] { "<token1>some-value</token2>", "some-value", XmlTokenType.Value, true };
            yield return new object[] { "<token-0/>  some-kinda-value    <token-2>", "some-kinda-value", XmlTokenType.Value, true };
            yield return new object[] { "</token1><not-a-token", null, XmlTokenType.None, false };
            yield return new object[] { "<token0>not-a>-token", null, XmlTokenType.None, false };
        }

        [Theory]
        [MemberData(nameof(ReadAftrerAdvancementTestCases))]
        public void Read_Advances_To_Next_Element(string xml, string tokenName, XmlTokenType tokenType, bool readResult)
        {
            WriteToBuffer(xml);

            reader.Read(buffer);

            Assert.Equal(reader.Read(buffer), readResult);
            VerifyReader(tokenName, tokenType);
        }

        [Fact]
        public void Read_Advances_After_Value_Token()
        {
            WriteToBuffer("<token>value</token>");
            reader.Read(buffer);
            VerifyReader("token", XmlTokenType.StartTag);
            reader.Read(buffer);
            VerifyReader("value", XmlTokenType.Value);

            reader.Read(buffer);
            VerifyReader("token", XmlTokenType.ClosingTag);
        }

        private void VerifyReader(string tokenName, XmlTokenType tokenType)
        {
            Assert.Equal(reader.TokenType, tokenType);
            Assert.Equal(reader.TokenName, tokenName);
        }

        private void WriteToBuffer(string xml)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
        }
    }
}