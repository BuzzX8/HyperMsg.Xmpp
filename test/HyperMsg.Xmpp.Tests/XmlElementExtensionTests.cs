using HyperMsg.Xmpp.Xml;
using System;
using System.Linq;

using Xunit;

namespace HyperMsg.Xmpp
{
    public class XmlElementExtensionsTests
    {
        [Fact]
        public void Attribute_ObjectValue_Updates_Attribute_Value()
        {
            var value = Guid.NewGuid();

            var element = new XmlElement("e").Attribute("attr", value);

            Assert.Equal(element["attr"], value.ToString());
        }

        [Fact]
        public void Attribute_StringValue_Updates_Attribute_Value()
        {
            var value = Guid.NewGuid().ToString();

            var element = new XmlElement("e").Attribute("attr", value);

            Assert.Equal(element["attr"], value);
        }

        [Fact]
        public void Child_Returns_Null_If_No_Does_Not_Contains_Child_With_specified_Name()
        {
            var element = new XmlElement("e", new XmlElement("child0"));

            Assert.Null(element.Child("child1"));
        }

        [Fact]
        public void Child_Returns_Xml_Element_With_Specified_Name_That_Is_Child_To_Current()
        {
            var child = new XmlElement("child1");
            var element = new XmlElement("root", child);

            Assert.Equal(element.Child(child.Name), child);
        }

        [Fact]
        public void Children_Adds_Set_Of_Elements_As_Child_Elements()
        {
            var children = Enumerable.Range(0, 3).Select(i => new XmlElement("e" + i)).ToArray();

            var element = new XmlElement("e").Children(children);

            Assert.Equal(children, element.Children);
        }

        [Fact]
        public void HasChild_Returns_False_If_No_Child_With_Specified_Name()
        {
            var element = new XmlElement("e", new XmlElement("child-1"));

            Assert.False(element.HasChild("child-0"));
        }

        [Fact]
        public void HasChild_Returns_True_If_Child_With_Specified_Name_Exists()
        {
            var element = new XmlElement("e", new XmlElement("child-0"));

            Assert.True(element.HasChild("child-0"));
        }

        [Fact]
        public void Value_Assigns_New_Value_To_Element()
        {
            var value = Guid.NewGuid().ToString();

            var element = new XmlElement("e").Value(value);

            Assert.Equal(element.Value, value);
        }

        [Fact]
        public void Xmlns_Returns_Value_Of_Xmlns_Attribute()
        {
            var element = new XmlElement("e");
            var value = Guid.NewGuid().ToString();
            element["xmlns"] = value;

            Assert.Equal(element.Xmlns(), value);
        }

        [Fact]
        public void Xmlns_Updates_Value_Of_Xmlns_Attribute()
        {
            var value = Guid.NewGuid().ToString();

            var element = new XmlElement("e").Xmlns(value);

            Assert.Equal(element["xmlns"], value);
        }

        [Fact]
        public void XmlLang_Returns_Value_Of_XmlLang_Attribute()
        {
            var element = new XmlElement("e");
            var value = Guid.NewGuid().ToString();
            element["xml:lang"] = value;

            Assert.Equal(element.XmlLang(), value);
        }

        [Fact]
        public void XmlLang_Updates_Value_Of_XmlLang_Attribute()
        {
            var value = Guid.NewGuid().ToString();

            var element = new XmlElement("e").XmlLang(value);

            Assert.Equal(element["xml:lang"], value);
        }
    }
}
