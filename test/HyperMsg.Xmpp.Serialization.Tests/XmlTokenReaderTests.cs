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

        

        private void WriteToBuffer(string xml)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
        }
    }
}