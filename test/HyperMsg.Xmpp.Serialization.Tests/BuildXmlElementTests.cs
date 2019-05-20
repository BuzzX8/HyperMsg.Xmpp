using System.Xml.Linq;
using Xunit;

namespace HyperMsg.Xmpp.Serialization
{
    public class BuildXmlElementTests
    {
        [Theory]
        [InlineData("<elem/>")]
        [InlineData("<iq type='get' from='me@home'/>")]
        [InlineData("<presence>Unavailable</presence>")]
        [InlineData("<message attr1='v1' attr2='d2'>Hello</message>")]
        [InlineData("<Parent><Child1/><Child2 /></Parent>")]
        [InlineData("<Parent><Child1 a1='v1'/><Child2><Subchild1/><Subchild2/></Child2></Parent>")]
        public void BuildXmlElement_Returns_Correct_Xml_Element(string xmlText)
        {
            var expectedElement = XElement.Parse(xmlText);
            var tokens = xmlText.GetTokens();

            XmlElement actualElement = tokens.BuildXmlElement();
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
            var tokens = xml.GetTokens();
            return tokens.BuildXmlElement();
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
