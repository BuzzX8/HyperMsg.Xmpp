using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg.Xmpp.Serialization
{
    public static class DeserializationExtensions
    {
        public static (int tokenSize, XmlToken token) ReadXmlToken(this ReadOnlySequence<byte> buffer)
        {
            var span = buffer.First.Span;
            int ltIndex = IndexOf(span, '<', 0);
            int gtIndex = IndexOf(span, '>', 0);

            if (!CanReadTokens(ltIndex, gtIndex) || buffer.Length == 0)
            {
                return (0, XmlToken.Empty);
            }

            if ((ltIndex > 0 && gtIndex < 0) || (gtIndex > 0 && ltIndex > 0))
            {
                var segment = GetBufferSegments(buffer, 0, ltIndex);
                return (ltIndex, new XmlToken(segment, XmlTokenType.Value));
            }

            if (span[ltIndex + 1] == '?' && span[gtIndex - 1] == '?')
            {
                var length = gtIndex - ltIndex + 1;
                var segment =  GetBufferSegments(buffer, ltIndex, length);
                return (length, new XmlToken(segment, XmlTokenType.Declaration));
            }

            int slashIndex = LastIndexOf(span, '/', ltIndex, gtIndex);

            if (IsStartTag(span, slashIndex, ltIndex, gtIndex))
            {
                var segment = GetBufferSegments(buffer, ltIndex, gtIndex - ltIndex + 1);
                return (gtIndex - ltIndex + 1, new XmlToken(segment, XmlTokenType.StartTag));
            }
            else if (IsNextWithoutSpaces(span, slashIndex, gtIndex))
            {
                var segment = GetBufferSegments(buffer, ltIndex, gtIndex + 1);
                return (gtIndex - ltIndex + 1, new XmlToken(segment, XmlTokenType.EnclosedTag));
            }
            else
            {
                var segment = GetBufferSegments(buffer, ltIndex, gtIndex + 1);
                return (gtIndex - ltIndex + 1, new XmlToken(segment, XmlTokenType.ClosingTag));
            }
        }

        private static bool  CanReadTokens(int ltIndex, int gtIndex)
        {
            if (gtIndex > 0 && (gtIndex < ltIndex || (ltIndex < 0)))
            {
                throw new DeserializationException();
            }
            
            if (ltIndex < 0 && gtIndex < 0)
            {
                return false;
            }

            return true;
        }

        private static ReadOnlySequence<byte> GetBufferSegments(ReadOnlySequence<byte> buffer, int start, int length)
        {
            return new ReadOnlySequence<byte>(buffer.First.Slice(start, length));
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

        public static DeserializationResult<XmlElement> ReadXmlElement(this ReadOnlySequence<byte> buffer)
        {
            (var size, var tokens) = buffer.ReadAvailableXmlTokens();

            if (!tokens.CanBuildXmlElement())
            {
                return new DeserializationResult<XmlElement>(0, default);
            }

            return new DeserializationResult<XmlElement>(size, tokens.BuildXmlElement());
        }

        public static (int, IEnumerable<XmlToken>) ReadAvailableXmlTokens(this ReadOnlySequence<byte> buffer)
        {
            var readings = new List<(int tokenSize, XmlToken token)>();

            while (buffer.Length > 0)
            {
                var reading = buffer.ReadXmlToken();

                if (reading.tokenSize == 0)
                {
                    break;
                }

                readings.Add(reading);
                buffer = buffer.Slice(reading.tokenSize, buffer.Length - reading.tokenSize);
            }
            

            return (readings.Select(r => r.tokenSize).Sum(), readings.Select(r => r.token));
        }
    }
}
