using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Serialization
{
    public class ReadAvailableXmlTokensTests
    {
        [Theory]
        [InlineData("<iq type='get' />")]
        [InlineData("<el1><el2>")]
        [InlineData("<element>value", "<element>")]
        [InlineData("<element>value</element>")]
        [InlineData("<parent><child/></parent>")]
        public void ReadAvailableXmlTokens_Returns_Correct_List_Of_XmlTokens(string xml, string expectedXml = null)
        {
            var buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(xml));
                        
            if (expectedXml == null)
            {
                expectedXml = xml;
            }

            var expectedReadLength = expectedXml.Length;
            var expectedTokens = expectedXml.GetTokens();

            (var actualReadLength, var actualTokens) = buffer.ReadAvailableXmlTokens();

            Assert.Equal(expectedReadLength, actualReadLength);
            AssertEqual(expectedTokens, actualTokens);
        }

        private void AssertEqual(IEnumerable<XmlToken> expected, IEnumerable<XmlToken> actual)
        {
            var exp = expected.Select(t => (t.Type, Encoding.UTF8.GetString(t.BufferSegments.ToArray())));
            var act = actual.Select(t => (t.Type, Encoding.UTF8.GetString(t.BufferSegments.ToArray())));

            Assert.Equal(exp, act);
        }
    }
}
