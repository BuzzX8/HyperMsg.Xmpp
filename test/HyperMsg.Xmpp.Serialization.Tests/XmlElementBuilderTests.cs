using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HyperMsg.Xmpp.Serialization.Tests
{
    public class XmlElementBuilderTests
    {
        private XmlElementBuilder builder = new XmlElementBuilder();
        private XmlLexer lexer = new XmlLexer(new XmlLexerOptions { ParseTagName = true });

        public static IEnumerable<object[]> GetTestElements()
        {
            yield return TestData("Enclosed element without attributes", new XElement("elem"));

            yield return TestData("Enclosed element with attributes", new XElement("iq",
                new XAttribute("type", "get"),
                new XAttribute("from", "me@home")));

            yield return TestData("Element with value and without attributes", new XElement("presence", "Unavaliable"));

            yield return TestData("Element with value and with attributes", new XElement("message", "Hello",
                new XAttribute("attr1", "v1"),
                new XAttribute("attr2", "d2")));

            yield return TestData("Element with childs", new XElement("Parent",
                new XElement("Child1"),
                new XElement("Child2")));

            yield return TestData("Elements whith childs and subchilds", new XElement("Parent",
                new XElement("Child1", new XAttribute("a1", "v1")),
                new XElement("Child2", new XElement("Subchild1"), new XElement("Subchild2"))));
        }

        private static object[] TestData(string name, XElement expected)
        {
            return new object[] { expected };
        }


        [Theory]
        [MemberData(nameof(GetTestElements))]
        public void VerifyBuilder(XElement expectedElement)
        {
            string xmlText = expectedElement.ToString();
            XmlElement actualElement = BuildElement(xmlText);
            AssertEqual(actualElement, expectedElement);
        }

        [Fact]
        public void Build_Builds_Closing_Element()
        {
            var element = BuildElement("</stream:stream>");

            Assert.Equal("/stream:stream", element.Name);
        }

        private XmlElement BuildElement(string xml)
        {
            var tokens = lexer.GetTokens(xml);
            return builder.Build(tokens);
        }

        private void AssertEqual(XmlElement actual, XElement expected)
        {
            Assert.Equal(actual.Name, expected.Name.ToString());

            actual.ForEachAttribute((name, value) =>
            {
                var expAttr = expected.Attribute(name);
                Assert.NotNull(expAttr);
                Assert.Equal(value, expAttr.Value);
            });

            if (!string.IsNullOrEmpty(expected.Value) && !string.IsNullOrEmpty(actual.Value?.ToString()))
            {
                Assert.Equal(actual.Value, expected.Value);
            }

            foreach (var child in actual.Children)
            {
                var expChild = expected.Element(child.Name);
                AssertEqual(child, expChild);
            }
        }
    }
}
