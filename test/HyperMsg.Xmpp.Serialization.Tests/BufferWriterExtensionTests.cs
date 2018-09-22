﻿using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace HyperMsg.Xmpp.Serialization.Tests
{
    public class BufferWriterExtensionTests
    {
        [Fact]
		public void Correctly_Writes_Element_With_Only_Name()
        {
            XmlElement element = new XmlElement("some-name");                      

            XElement actualElement = GetSerializedElement(element);
            Assert.Equal(actualElement.Name.LocalName, ("some-name"));
            Assert.Equal(actualElement.Attributes().Count(), (0));
            Assert.Equal(actualElement.Elements().Count(), (0));
        }

        [Fact]
        public void Correctly_Writes_Element_Name_And_Xmlns()
        {
            XmlElement element = new XmlElement("some-name").Xmlns("name-space");
            
            XElement actualElement = GetSerializedElement(element);
            Assert.Equal(actualElement.Name.LocalName, ("some-name"));
            Assert.Equal(actualElement.Attribute("xmlns").Value, ("name-space"));
        }

        [Fact]
        public void Correctly_Writes_Attributes()
        {
            XmlElement element = new XmlElement("element");
            element.SetAttributeValue("attribute1", "value1");
            element.SetAttributeValue("attribute2", 2);
            
            XElement actualElement = GetSerializedElement(element);
            Assert.Equal(actualElement.Attributes().Count(), (2));
            Assert.Equal(actualElement.Attribute("attribute1").Value, ("value1"));
            Assert.Equal(actualElement.Attribute("attribute2").Value, ("2"));
        }

        [Fact]
        public void Correctly_Writes_Xmlns_And_Elements_Value()
        {
            XmlElement element = new XmlElement("name")
                .Xmlns("namespace")
                .Value("Some-Value");
                        
            XElement actualElement = GetSerializedElement(element);
            Assert.Equal(actualElement.Value, ("Some-Value"));
        }

        [Fact]
        public void Correctly_Writes_Child_Elements()
        {
            XmlElement rootElement = new XmlElement("root");
            XmlElement element1 = new XmlElement("element1").Value("value1");
            XmlElement element2 = new XmlElement("element2").Value("value2");
            rootElement.Children.Add(element1);
            rootElement.Children.Add(element2);
                        
            XElement actualElement = GetSerializedElement(rootElement);
            Assert.Equal(actualElement.Element("element1").Value, ("value1"));
            Assert.Equal(actualElement.Element("element2").Value, ("value2"));
        }

        //[Fact]
        //public void Correctly_Writes_StreamHeader()
        //{
        //    var header = StreamHeader.Client().To("me@home.com");

        //    serializer.Serialize(write, header);

        //    var data = stream.ToArray();
        //    XmlToken[] tokens = new XmlLexer().GetTokens(Encoding.UTF8.GetString(data));
        //    Assert.Equal(tokens[0].Type, (XmlTokenType.Declaration));
        //    Assert.Equal(tokens[1].Type, (XmlTokenType.StartTag));
        //}

        private XElement GetSerializedElement(XmlElement element)
        {
            var pipe = new Pipe();
            pipe.Writer.WriteXmlElement(element);
            pipe.Writer.FlushAsync().AsTask().Wait();
            var result = pipe.Reader.ReadAsync().Result.Buffer.First.ToArray();

            return XElement.Parse(Encoding.UTF8.GetString(result));
        }
    }
}