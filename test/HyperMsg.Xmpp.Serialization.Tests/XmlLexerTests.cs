using System;
using System.Linq;
using Xunit;

namespace HyperMsg.Xmpp.Serialization.Tests
{
    public class XmlLexerTests
    {
        private XmlLexer lexer = new XmlLexer();

        [Fact(DisplayName = "GetTokens parses xml declaration")]
        public void GetTokens_Correctly_Parses_Xml_Declaration()
        {
            string declaration = "<?xml version='1.0'?>";

            var token = lexer.GetTokens(declaration).Single();

            Assert.Equal(token.Type, (XmlTokenType.Declaration));
        }

        [Fact(DisplayName = "GetTokens parses start tags")]
        public void GetTokens_Correctly_Parses_Start_Tags()
        {
            string element = "<stream:stream xmlns='jabber:client' id='3744742480'>";
            var token = lexer.GetTokens(element).Single();

            Assert.Equal(token.Type, (XmlTokenType.StartTag));
            Assert.Equal(XmlLexer.GetTagName(token), ("stream:stream"));
        }

        [InlineData("<element />")]
        [InlineData("<ns:element />")]
        [InlineData("<c xmlns='http://jabber.org/protocol/caps' hash='sha-1' />")]
        [Theory(DisplayName = "GetTokens parses enclosed tag")]
        public void GetTokens_Correctly_Parses_Enclosed_Tag(string tag)
        {
            var tokens = lexer.GetTokens(tag).ToArray();
            Assert.Equal(tokens.Length, (1));
            Assert.Equal(tokens[0].Type, (XmlTokenType.EnclosedTag));
        }

        [InlineData("</ elem>")]
        [InlineData("</some-elem>")]
        [InlineData("</stream:features>")]
        [Theory(DisplayName = "GetTokens parses closing tags")]
        public void GetTokens_Correctly_Parses_Closing_Tags(string tag)
        {
            var token = lexer.GetTokens(tag).Single();
            Assert.Equal(token.Type, (XmlTokenType.ClosingTag));
        }

        [InlineData("<node>Value</node>")]
        [InlineData("<node attr1='val0' attr2='val'>Value</node>")]
        [Theory(DisplayName = "GetTokens parses elements with values")]
        public void GetTokens_Correctly_Parses_Element_With_Value(string element)
        {
            var tokens = lexer.GetTokens(element).ToArray();

            Assert.Equal(tokens.Length, (3));
            Assert.Equal(tokens[0].Type, (XmlTokenType.StartTag));
            Assert.Equal(tokens[1].Type, (XmlTokenType.Value));
            Assert.Equal(tokens[1].Value, ("Value"));
            Assert.Equal(tokens[2].Type, (XmlTokenType.ClosingTag));
        }

        [Fact(DisplayName = "GetTokens parses element with childs")]
        public void GetTokens_Correctly_Parses_Element_With_Childs()
        {
            string element = @"<root ><child1 /><child2>Fer kol</ child0><child3 atr=""1"" art='rtr'/></root>";

            var tokens = lexer.GetTokens(element).ToArray();

            Assert.Equal(tokens.Length, (7));
            AssertToken(tokens[0], XmlTokenType.StartTag, "root");
            AssertToken(tokens[1], XmlTokenType.EnclosedTag, "child1");
            AssertToken(tokens[2], XmlTokenType.StartTag, "child2");
            AssertToken(tokens[3], XmlTokenType.Value, "Fer kol");
            AssertToken(tokens[4], XmlTokenType.ClosingTag, "child0");
            AssertToken(tokens[5], XmlTokenType.EnclosedTag, "child3");
            AssertToken(tokens[6], XmlTokenType.ClosingTag, "root");
        }

        private void AssertToken(XmlToken token, XmlTokenType type, string nameOrValue)
        {
            Assert.Equal(token.Type, (type));
            if (token.Type != XmlTokenType.Value) Assert.Equal(XmlLexer.GetTagName(token), (nameOrValue));
            else Assert.Equal(token.Value, (nameOrValue));
        }

        [Fact(DisplayName = "GetTokens parses whitespaces")]
        public void GetTokens_Correctly_Parses_Whitespaces()
        {
            string xml = "\t<element/>\n <a>Val</a> ";

            var tokens = lexer.GetTokens(xml).ToArray();

            Assert.Equal(tokens[0].Type, (XmlTokenType.Whitespace));
            Assert.Equal(tokens[2].Type, (XmlTokenType.Whitespace));
            Assert.Equal(tokens[6].Type, (XmlTokenType.Whitespace));
        }

        [Fact(DisplayName = "GetTokens parses tag names")]
        public void GetTokens_Parses_Tag_Names_If_ParseTagName_Is_True()
        {
            lexer.Options.ParseTagName = true;
            var token = lexer.GetTokens("<some-element>").Single();

            Assert.Equal(token.TagName, ("some-element"));
        }

        [Fact(DisplayName = "GetTokens ignores whitespaces")]
        public void GetTokens_Ignores_Whitespaces_If_IgnoreWhitespaces_Is_True()
        {
            string xml = "<Item1>\n</Item2>";
            lexer.Options.IgnoreWhitespaces = true;

            var tokens = lexer.GetTokens(xml).ToArray();

            Assert.Equal(tokens.Length, (2));
            Assert.Equal(tokens[0].Type, (XmlTokenType.StartTag));
            Assert.Equal(tokens[0].Value, ("<Item1>"));
            Assert.Equal(tokens[1].Type, (XmlTokenType.ClosingTag));
            Assert.Equal(tokens[1].Value, ("</Item2>"));
        }

        [Fact(DisplayName = "Does not parses tag names for non tag tokens")]
        public void Does_Not_Parses_TagName_For_Non_Tag_Tokens()
        {
            lexer.Options.ParseTagName = true;
            var value = lexer.GetTokens("<e>value</e>")[1];

            Assert.Null(value.TagName);
        }

        [InlineData(@"<kitty />", "kitty")]
        [InlineData(@"<k:itty>", "k:itty")]
        [InlineData(@"<kitt-y8 attr1='val1' attr0='val0'>", "kitt-y8")]
        [Theory(DisplayName = "GetTokens returns tag name")]
        public void GetTagName_Returns_Tag_Name(string xml, string expectedName)
        {
            XmlToken token = lexer.GetTokens(xml).Single();

            string actualName = XmlLexer.GetTagName(token);

            Assert.Equal(expectedName, actualName);
        }

        [Fact(DisplayName = "GetTagAttributes returns empty collection for tag without attributes")]
        public void GetTagAttributes_Returns_Empty_Collection_For_Tag_Without_Attributes()
        {
            XmlToken token = lexer.GetTokens("<someone >").Single();

            var attributes = XmlLexer.GetTagAttributes(token);

            Assert.Equal(attributes.Count(), (0));
        }

        [Fact(DisplayName = "GetTagAttributes returns collection of attributes")]
        public void GetTagAttributes_Returns_Correct_Collection_Of_Attributes()
        {
            var expectedAttributes = Enumerable.Range(1, 10)
                .Select(i => new Tuple<string, string>($"attr{i}", $"val{i}"));
            string tag = string.Concat(expectedAttributes.Select(a => $"{a.Item1}='{a.Item2}' "));
            tag = $"<tag {tag}/>";
            var token = lexer.GetTokens(tag).Single();

            var actualAttributes = XmlLexer.GetTagAttributes(token).ToArray();

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
            XmlToken token = lexer.GetToken($"<item {name}='{value}' />");

            var actualAttribute = XmlLexer.GetTagAttributes(token).Single();

            Assert.Equal(actualAttribute.Item1, name);
            Assert.Equal(actualAttribute.Item2, value);
        }
    }
}
