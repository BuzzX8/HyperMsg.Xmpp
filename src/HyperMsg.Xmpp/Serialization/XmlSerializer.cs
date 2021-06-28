using HyperMsg.Xmpp.Xml;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
    public static class XmlSerializer
    {
        public static void Serialize(IBufferWriter writer, XmlElement element)
        {
            string closingChars = " />";

            if (IsStreamHeader(element))
            {
                WriteDeclaration(writer);
                closingChars = ">";
            }

            if (element.Name.StartsWith("/"))
            {
                closingChars = ">";
            }

            WriteElementStart(writer, element.Name);
            WriteAttributes(writer, element);

            if (element.HasChildren || !string.IsNullOrEmpty(element.Value))
            {
                WriteContent(writer, element);
            }
            else
            {
                WriteText(writer, closingChars);
            }
        }

        private static bool IsStreamHeader(XmlElement element)
        {
            return element.Name == "stream:stream";
        }

        private static void WriteDeclaration(IBufferWriter writer)
        {
            WriteText(writer, "<?xml version='1.0' ?>");
        }

        private static void WriteElementStart(IBufferWriter writer, string name)
        {
            WriteText(writer, "<");
            WriteText(writer, name);
        }

        private static void WriteAttributes(IBufferWriter writer, XmlElement element)
        {
            element.ForEachAttribute((name, value) =>
            {
                WriteText(writer, " ");
                WriteText(writer, name);
                WriteText(writer, "='");
                WriteText(writer, value);
                WriteText(writer, "'");
            });
        }

        private static void WriteContent(IBufferWriter writer, XmlElement element)
        {
            WriteText(writer, ">");
            WriteValue(writer, element.Value);
            WriteChilds(writer, element);
            WriteClosingTag(writer, element.Name);
        }

        private static void WriteChilds(IBufferWriter writer, XmlElement element)
        {
            if (element.HasChildren)
            {
                foreach (var child in element.Children)
                {
                    Serialize(writer, child);
                }
            }
        }

        private static void WriteClosingTag(IBufferWriter writer, string name)
        {
            WriteText(writer, "</");
            WriteText(writer, name);
            WriteText(writer, ">");
        }

        private static void WriteText(IBufferWriter writer, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            writer.Write(bytes);
        }

        private static void WriteValue(IBufferWriter writer, object value)
        {
            if (value == null)
            {
                return;
            }

            WriteText(writer, value.ToString());
        }
    }
}
