using HyperMsg.Xmpp.Xml;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
    public static class XmlTokenExtensions
    {
        public static bool CanBuildXmlElement(this IEnumerable<XmlToken> tokens)
        {
            var openTags = new Stack<(string name, XmlToken)>();

            foreach (var token in tokens)
            {
                var xml = GetXmlString(token);

                switch (token.Type)
                {
                    case XmlTokenType.ClosingTag:
                        var prev = openTags.Peek();                        

                        if (prev.name == xml.GetTagName())
                        {
                            openTags.Pop();
                        }
                        else
                        {
                            throw new FormatException();
                        }

                        if (openTags.Count == 0)
                        {
                            return true;
                        }

                        continue;

                    case XmlTokenType.EnclosedTag:
                        if (openTags.Count == 0)
                        {
                            return true;
                        }
                        continue;

                    case XmlTokenType.StartTag:                        
                        openTags.Push((xml.GetTagName(), token));
                        continue;
                }
            }

            return false;
        }

        public static XmlElement BuildXmlElement(this IEnumerable<XmlToken> tokens)
        {
            XmlElement element = null;
            Stack<XmlElement> parents = null;

            foreach (var token in tokens)
            {
                var xml = GetXmlString(token);

                switch (token.Type)
                {
                    case XmlTokenType.StartTag:
                        BuildStartTag(xml, ref element, ref parents);
                        continue;

                    case XmlTokenType.ClosingTag:
                        BuildClosingTag(xml, ref element, parents);
                        continue;

                    case XmlTokenType.EnclosedTag:
                        BuildEnclosedElement(xml, ref element);
                        continue;

                    case XmlTokenType.Value:
                        element.Value = xml;
                        continue;

                    default:
                        throw new NotSupportedException();
                }
            }

            return element;
        }

        private static void BuildEnclosedElement(string xml, ref XmlElement element)
        {
            var encElement = CreateElement(xml);

            if (element != null)
            {
                element.Children.Add(encElement);
            }
            else
            {
                element = encElement;
            }
        }

        private static void BuildStartTag(string xml, ref XmlElement element, ref Stack<XmlElement> parents)
        {
            if (element != null)
            {
                if (parents == null)
                {
                    parents = new Stack<XmlElement>();
                }

                var child = CreateElement(xml);
                element.Children.Add(child);
                parents.Push(element);
                element = child;
            }
            else
            {
                element = CreateElement(xml);
            }
        }

        private static void BuildClosingTag(string xml, ref XmlElement element, Stack<XmlElement> parents)
        {
            if (parents != null && parents.Count > 0)
            {
                element = parents.Pop();
            }

            if (element == null)
            {
                element = new XmlElement('/' + xml.GetTagName());
            }
        }

        private static XmlElement CreateElement(string xml)
        {
            var name = xml.GetTagName();
            var element = new XmlElement(name);
            AddAttributes(element, xml);
            return element;
        }

        private static void AddAttributes(XmlElement element, string xml)
        {
            foreach (var (name, value) in xml.GetTagAttributes())
            {
                element.SetAttributeValue(name, value);
            }
        }

        private static string GetXmlString(XmlToken token) => Encoding.UTF8.GetString(token.BufferSegments.ToArray());
    }
}
