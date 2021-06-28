using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg.Xmpp.Xml
{
    public class XmlElementTests
    {
        [Fact]
        public void Constructor_Correctly_Sets_Name()
        {
            string name = Guid.NewGuid().ToString();
            XmlElement element = new XmlElement(name);

            Assert.Equal(element.Name, name);
        }

        [Fact]
        public void GetAttributeValue_Returns_Null_If_No_Attribute_With_Given_Name()
        {
            XmlElement element = new XmlElement("e");

            Assert.Null(element.GetAttributeValue("attr"));
        }

        [Fact]
        public void SetAttributeValue_ObjectValue_Create_Attribute_With_Given_Value()
        {
            XmlElement element = new XmlElement("e");
            var value = Guid.NewGuid();

            element.SetAttributeValue("attr", value);

            Assert.Equal(element.GetAttributeValue("attr"), value.ToString());
        }

        [Fact]
        public void SetAttributeValue_StringValue_Creates_Attribute_With_Given_Value()
        {
            XmlElement element = new XmlElement("e");
            var value = Guid.NewGuid().ToString();

            element.SetAttributeValue("attr", value);

            Assert.Equal(element.GetAttributeValue("attr"), value);
        }

        [Fact]
        public void SetAttributeValue_Removes_Attribute_If_Value_Is_Null()
        {
            XmlElement element = new XmlElement("e");
            var value = Guid.NewGuid().ToString();
            element["attr"] = value;

            element.SetAttributeValue("attr", null);

            Assert.False(element.HasAttribute("attr"));
        }

        [Fact]
        public void Indexer_Returns_Null_If_No_attribute_With_Given_Name()
        {
            XmlElement element = new XmlElement("e");

            Assert.Null(element["attr"]);
        }

        [Fact]
        public void Indexer_Returns_Attribute_Value()
        {
            XmlElement element = new XmlElement("e");
            var value = Guid.NewGuid().ToString();

            element["attr"] = value;

            Assert.Equal(element["attr"], value);
        }

        [Fact]
        public void HasAttribute_Returns_False_If_No_Attribute_With_Given_Name()
        {
            XmlElement element = new XmlElement("e");

            Assert.False(element.HasAttribute("attr"));
        }

        [Fact]
        public void HasAttribute_Returns_True_If_Given_Attribute_Presents()
        {
            XmlElement element = new XmlElement("e")
            {
                ["attr"] = "value"
            };

            Assert.True(element.HasAttribute("attr"));
        }

        [Fact]
        public void HasAttributes_Returns_True_If_Element_Has_Attributes()
        {
            XmlElement element = new XmlElement("e");
            Assert.False(element.HasAttributes);

            element["attr"] = "some-value";

            Assert.True(element.HasAttributes);
        }

        [Fact]
        public void ForEachAttribute_Iterates_Over_Each_Attribute()
        {
            var expectedAttributes = new Dictionary<string, object>
            {
                { "value1", "1" },
                { "value2", "value" }
            };
            XmlElement element = new XmlElement("element");

            foreach (var attribute in expectedAttributes)
            {
                element.SetAttributeValue(attribute.Key, attribute.Value);
            }

            var actualAttributes = new Dictionary<string, object>();

            element.ForEachAttribute((n, v) => actualAttributes.Add(n, v));

            Assert.Equal(actualAttributes, expectedAttributes);
        }

        [Fact]
        public void HasChildren_Return_False_If_No_Children()
        {
            XmlElement element = new XmlElement("e");

            Assert.Equal(element.Children.Count, 0);
            Assert.False(element.HasChildren);
        }

        [Fact]
        public void HasChildren_Return_True_If_Element_Has_Children()
        {
            XmlElement element = new XmlElement("e",
                new XmlElement("child-element"));

            Assert.True(element.HasChildren);
        }

        public static IEnumerable<object[]> EqualsTestCases()
        {
            yield return new object[] { new XmlElement("e"), null, false };
            yield return new object[] { new XmlElement("e"), "some-string", false };
            yield return new object[] { new XmlElement("elem"), new XmlElement("elem"), true };
            yield return new object[] { new XmlElement("elem1"), new XmlElement("elem2"), false };

            yield return new object[]
            {
                new XmlElement("elem"),
                new XmlElement("elem").Attribute("attr", "val"), false
            };

            yield return new object[]
            {
                new XmlElement("e").Attribute("atr", "val1"),
                new XmlElement("e"), false
            };

            yield return new object[]
            {
                new XmlElement("e").Attribute("atr", "val0"),
                new XmlElement("e").Attribute("atr", "val0"), true
            };

            yield return new object[]
            {
                new XmlElement("e").Attribute("atr", "val0"),
                new XmlElement("e").Attribute("atr", "val1"), false
            };

            yield return new object[]
            {
                new XmlElement("e").Value("value1"),
                new XmlElement("e").Value("value2"), false
            };

            yield return new object[]
            {
                new XmlElement("e").Value("value"),
                new XmlElement("e").Value("value"), true
            };

            yield return new object[]
            {
                new XmlElement("parent", new XmlElement("child")),
                new XmlElement("parent"), false
            };

            yield return new object[]
            {
                new XmlElement("parent", new XmlElement("child")),
                new XmlElement("parent", new XmlElement("child")), true
            };
        }

        [Theory]
        [MemberData(nameof(EqualsTestCases))]
        public void Equals_Returns_Expected_Result(XmlElement element, object other, bool expectedResult)
        {
            Assert.Equal(element.Equals(other), expectedResult);
        }
    }
}