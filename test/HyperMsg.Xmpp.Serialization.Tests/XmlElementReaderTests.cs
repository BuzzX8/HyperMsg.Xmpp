using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp.Serialization.Tests
{

    public class XmlElementReaderTests
    {
        private ArraySegment<byte> buffer;
        private XmlElementReader reader;

        public XmlElementReaderTests()
        {
            buffer = new ArraySegment<byte>();
            reader = new XmlElementReader();
        }

        public static IEnumerable<object[]> HasElementTestCases()
        {
            yield return new object[] { "<some-element />", true };
            yield return new object[] { "<some-element>", false };
            yield return new object[] { "</elem>", false };
            yield return new object[] { "<element></element>", true };
            yield return new object[] { "<element1></element2>", false };
            yield return new object[] { "<element>value</element>", true };
            yield return new object[] { "<element>value</element2>", false };
            yield return new object[] { "<root><child></child></root>", true };
            yield return new object[] { "<root><child></child>", false };
            //yield return new object[] { "<root><child></child0></root>", false };
            yield return new object[] { "<root><child /></root>", true };
            yield return new object[] { "<root><child />", false };
            yield return new object[] { "<root><child>value</child></root>", true };
            yield return new object[] { "<stream:stream>", true };
            yield return new object[] { "</stream:stream>", true };
            yield return new object[] { "<?xml version='1.0'?><stream:stream>", true };
            yield return new object[]
            {
                @"<root>
					<child1 />
					<child2 />
				  </root>",
                true
            };
            yield return new object[]
            {
                @"<root>
					<child1 />
					<child2>Value2</child2>
				  </root>",
                true
            };
            yield return new object[]
            {
                @"<root>
					<child1>Value1</child1>
					<child2>Value2</child2>
				  </root>",
                true
            };
        }

        [Theory(DisplayName = "HasElement")]
        [MemberData(nameof(HasElementTestCases))]
        public void HasElement_Returns_True_If_Buffer_Caontains_XmlElement(string xml, bool expected)
        {
            WriteToBuffer(xml);

            Assert.Equal(reader.HasElement(buffer), expected);
        }

        public static IEnumerable<object[]> ReadTestCases()
        {
            yield return new object[] { "<element/>", "<element/>" };
            yield return new object[] { "<origin /><other/>", "<origin />" };
            yield return new object[] { "<elem> </elem></other-elem>", "<elem> </elem>" };
            yield return new object[] { "<elem>val</elem><some-other-one>", "<elem>val</elem>" };
            yield return new object[] { "<root><child/></root><other-elem>", "<root><child/></root>" };
            yield return new object[] { "<root><child></child></root><other-elem>", "<root><child></child></root>" };
            yield return new object[] { "<root><child>val</child></root><other-elem>", "<root><child>val</child></root>" };
            yield return new object[] { "<stream:stream><some-other>", "<stream:stream>" };
            yield return new object[] { "</stream:stream><some-other>", "</stream:stream>" };
            yield return new object[] { "<?xml version='1.0'?><stream:stream><some-other>", "<?xml version='1.0'?><stream:stream>" };
            yield return new object[]
            {
                @"<root>
					<child1>Value1</child1>
					<child2>Value2</child2>
				  </root>",
                @"<root>
					<child1>Value1</child1>
					<child2>Value2</child2>
				  </root>"
            };
        }

        [Theory(DisplayName = "Read")]
        [MemberData(nameof(ReadTestCases))]
        public void Read_Returns_Correct_Xml_Element_From_Buffer(string xml, string expectedXml)
        {
            WriteToBuffer(xml);

            string actualXml = reader.ReadElement(buffer);

            Assert.Equal(actualXml, expectedXml);
        }

        public static IEnumerable<object[]> ReadThrowsExceptionTestCases()
        {
            yield return new object[] { "<element>" };
            yield return new object[] { "<element></element2>" };
            yield return new object[] { "</element>" };
        }

        [Theory(DisplayName = "Read exception")]
        [MemberData(nameof(ReadThrowsExceptionTestCases))]
        public void Read_Throws_Exception_If_Invalid_Element_In_Buffer(string xml)
        {
            WriteToBuffer(xml);

            Assert.Throws<Exception>(() => reader.ReadElement(buffer));
        }

        private void WriteToBuffer(string xml)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
        }
    }
}
