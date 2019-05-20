using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Serialization
{
    public class ReadAvailableXmlTokensTests
    {
        public static IEnumerable<object[]> GetTestCases()
        {
            //yield return GetTestCase("<el1>", "<el2 />");
            yield return GetTestCase("<e1>", "value");
        }

        private static object[] GetTestCase(params string[] tokens)
        {
            var fullXml = tokens.Aggregate((a, t) => a + t);
            var buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(fullXml));

            var expectedReadLength = buffer.Length;

            var expectedTokens = tokens.Select(t =>
            {
                var segment = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(t));
                var tokenType = XmlStringExtensions.GetTokenType(t);
                return new XmlToken(segment, tokenType);
            });

            return new object[] { buffer, expectedReadLength, expectedTokens };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void ReadAvailableXmlTokens_Returns_Correct_List_Of_XmlTokens(ReadOnlySequence<byte> buffer, int expectedReadLength, IEnumerable<XmlToken> expectedTokens)
        {
            (var actualReadLength, var actualTokens) = buffer.ReadAvailableXmlTokens();

            Assert.Equal(expectedReadLength, actualReadLength);
            Assert.Equal(expectedTokens, actualTokens);
        }
    }
}
