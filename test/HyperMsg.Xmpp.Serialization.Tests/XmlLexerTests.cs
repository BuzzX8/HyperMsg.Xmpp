using System;
using System.Linq;
using Xunit;

namespace HyperMsg.Xmpp.Serialization.Tests
{
    public class XmlLexerTests
    {
        [InlineData(@"<kitty />", "kitty")]
        [InlineData(@"<k:itty>", "k:itty")]
        [InlineData(@"<kitt-y8 attr1='val1' attr0='val0'>", "kitt-y8")]
        [Theory(DisplayName = "GetTokens returns tag name")]
        public void GetTagName_Returns_Tag_Name(string xml, string expectedName)
        {
            string actualName = xml.GetTagName();

            Assert.Equal(expectedName, actualName);
        }

        [Fact(DisplayName = "GetTagAttributes returns empty collection for tag without attributes")]
        public void GetTagAttributes_Returns_Empty_Collection_For_Tag_Without_Attributes()
        {
            var attributes = "<someone >".GetTagAttributes();

            Assert.Equal(attributes.Count(), (0));
        }

        [Fact(DisplayName = "GetTagAttributes returns collection of attributes")]
        public void GetTagAttributes_Returns_Correct_Collection_Of_Attributes()
        {
            var expectedAttributes = Enumerable.Range(1, 10).Select(i => ($"attr{i}", $"val{i}"));
            string tag = $"<tag {string.Concat(expectedAttributes.Select(a => $"{a.Item1}='{a.Item2}' "))}/>";            

            var actualAttributes = tag.GetTagAttributes();

            Assert.Equal(actualAttributes, expectedAttributes);
        }

        [InlineData("void", "")]
        [InlineData("attr-2", "8")]
        [InlineData("d:tr", "12-1")]
        [InlineData("attr1", "val1")]
        [InlineData("jid", "user@home")]
        [InlineData("xmlns:stream", "http://etherx.jabber.org/streams")]
        [Theory(DisplayName = "GetTagAttributes parses attributes")]
        public void GetTagAttribute_Correctly_Parses_Attributes(string name, string value)
        {
            var actualAttribute = $"<item {name}='{value}' />".GetTagAttributes().Single();

            Assert.Equal(actualAttribute.Item1, name);
            Assert.Equal(actualAttribute.Item2, value);
        }
    }
}
