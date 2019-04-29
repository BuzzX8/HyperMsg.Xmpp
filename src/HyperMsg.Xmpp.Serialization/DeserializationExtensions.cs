using System;
using System.Buffers;

namespace HyperMsg.Xmpp.Serialization
{
    public static class DeserializationExtensions
    {
        public static (int tokenSize, XmlToken token) ReadXmlToken(this ReadOnlySequence<byte> buffer)
        {
            var span = buffer.First.Span;
            int ltIndex = IndexOf(span, '<', 0);
            int gtIndex;

            if (ltIndex < 0 || (gtIndex = IndexOf(span, '>', ltIndex)) < 0)
            {
                return (0, new XmlToken(XmlTokenType.None, string.Empty));
            }

            if (span[ltIndex + 1] == '?' && span[gtIndex - 1] == '?')
            {
                return (0, new XmlToken(XmlTokenType.Declaration, string.Empty));
            }

            if (ltIndex > 1 && !IsNextWithoutSpaces(span, 0, ltIndex))
            {
                //UpdateNameAndType(span, 1, ltIndex - 1, XmlTokenType.Value);                
                return (0, new XmlToken(XmlTokenType.Value, string.Empty));
            }

            int slashIndex = LastIndexOf(span, '/', ltIndex, gtIndex);

            if (IsStartTag(span, slashIndex, ltIndex, gtIndex))
            {
                //UpdateNameAndType(span, ltIndex + 1, gtIndex - ltIndex - 1, XmlTokenType.StartTag);
            }
            else if (IsNextWithoutSpaces(span, slashIndex, gtIndex))
            {
                //UpdateNameAndType(span, ltIndex + 1, slashIndex - ltIndex - 1, XmlTokenType.EnclosedTag);
            }
            else
            {
                //UpdateNameAndType(span, slashIndex + 1, gtIndex - slashIndex - 1, XmlTokenType.ClosingTag);
            }

            throw new NotImplementedException();
        }

        private static bool IsNextWithoutSpaces(ReadOnlySpan<byte> buffer, int startIndex, int endIndex)
        {
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                if (!char.IsWhiteSpace((char)buffer[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsStartTag(ReadOnlySpan<byte> buffer, int slashIndex, int ltIndex, int gtIndex)
        {
            return slashIndex == -1
                || (!IsNextWithoutSpaces(buffer, slashIndex, gtIndex)
                && !IsNextWithoutSpaces(buffer, ltIndex, slashIndex));
        }

        private static int IndexOf(ReadOnlySpan<byte> buffer, char c, int startIndex)
        {
            for (int i = startIndex; i < buffer.Length; i++)
            {
                if (buffer[i] == c)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int LastIndexOf(ReadOnlySpan<byte> buffer, char c, int startIndex, int endIndex)
        {
            for (int i = endIndex; i >= startIndex; i--)
            {
                if (buffer[i] == c)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
