using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
    public static class DeserializationExtensions
    {
        public static (int tokenSize, XmlToken token) ReadXmlToken(this ReadOnlySequence<byte> buffer)
        {
            var span = buffer.First.Span;
            int ltIndex = IndexOf(span, '<', 0);
            int gtIndex;

            if (!HasValidTokens(span))
            {
                throw new DeserializationException();
            }

            if (ltIndex < 0 || (gtIndex = IndexOf(span, '>', ltIndex)) < 0)
            {
                return (0, new XmlToken(XmlTokenType.None, string.Empty));
            }

            if (span[ltIndex + 1] == '?' && span[gtIndex - 1] == '?')
            {
                return (gtIndex - ltIndex + 1, new XmlToken(XmlTokenType.Declaration, string.Empty));
            }

            if (ltIndex > 1 && !IsNextWithoutSpaces(span, 0, ltIndex))
            {
                var name = GetTokenName(span, 1, ltIndex - 1);
                return (ltIndex - 1, new XmlToken(XmlTokenType.Value, name));
            }

            int slashIndex = LastIndexOf(span, '/', ltIndex, gtIndex);

            if (IsStartTag(span, slashIndex, ltIndex, gtIndex))
            {
                var name = GetTokenName(span, ltIndex + 1, gtIndex - ltIndex - 1);
                return (gtIndex - ltIndex + 1, new XmlToken(XmlTokenType.StartTag, name));
            }
            else if (IsNextWithoutSpaces(span, slashIndex, gtIndex))
            {
                var name = GetTokenName(span, ltIndex + 1, slashIndex - ltIndex - 1);
                return (gtIndex - ltIndex + 1, new XmlToken(XmlTokenType.EnclosedTag, name));
            }
            else
            {
                var name = GetTokenName(span, slashIndex + 1, gtIndex - slashIndex - 1);
                return (gtIndex - ltIndex + 1, new XmlToken(XmlTokenType.ClosingTag, name));
            }
        }

        private static bool HasValidTokens(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length == 0)
            {
                return false;
            }

            int ltIndex = IndexOf(buffer, '<', 0);

            return ltIndex >= 0 && IndexOf(buffer, '>', ltIndex) >= 0;
        }

        private static string GetTokenName(ReadOnlySpan<byte> buffer, int startIndex, int count)
        {
            var str = Encoding.UTF8.GetString(buffer.ToArray(), startIndex, count);
            return str.Trim().Split(' ')[0];
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

        public static (int, XmlElement) ReadXmlElement(this ReadOnlySequence<byte> buffer)
        {
            var tokens = buffer.ReadAvailableXmlTokens();

            if (tokens.CanBuildXmlElement())
            {
                return (0, null);
            }

            throw new NotImplementedException();
        }

        public static IEnumerable<XmlToken> ReadAvailableXmlTokens(this ReadOnlySequence<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}
