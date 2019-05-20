using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyperMsg.Xmpp.Serialization
{
    public static class XmlStringExtensions
    {
        private static readonly string XmlDeclarationRegexp = @"\<\?xml[^\<\>\\]*\?\>";
        private static readonly string XmlEnclosedTagRegex = @"\<[\w-\:]+[^\<\>\\]*\/\>";
        private static readonly string XmlStartTagRegex = @"\<[\w-\:]+[^\<\>]*[^\\]\>";
        private static readonly string XmlClosingTagRegex = @"\<\/\s*[\w-\:]+\>";
        private static readonly string XmlValueRegex = @"[^\<\>\\]+";
        private static readonly string WhitespaceRegex = @"^[\s\t]+";
        private static readonly string GenericXmlElementRegex = string.Format(@"({0})|({1})|({2})|({3})|({4})", XmlDeclarationRegexp, XmlStartTagRegex, XmlClosingTagRegex, XmlEnclosedTagRegex, XmlValueRegex);

        private static readonly string XmlAttributeRegex = @"[\w-\:]+=('|"")[\S]*\1";

        public static IEnumerable<XmlToken> GetTokens(this string xmlFragment, bool ignoreWhitespaces = true)
        {
            var matches = Regex.Matches(xmlFragment, GenericXmlElementRegex);

            for (int i = 0; i < matches.Count; i++)
            {
                string value = matches[i].Value;

                if (IsWhitespace(value) && ignoreWhitespaces)
                {
                    continue;
                }

                yield return GetToken(value);
            }
        }

        public static XmlToken GetToken(this string xml)
        {
            var tokenType = GetTokenType(xml);
            var bytes = Encoding.UTF8.GetBytes(xml);

            return new XmlToken(new ReadOnlySequence<byte>(bytes), tokenType);
        }

        public static IEnumerable<(string name, string value)> GetTagAttributes(this string xmlTag)
        {
            var attributes = Regex.Matches(xmlTag, XmlAttributeRegex)
                                  .Cast<Match>()
                                  .Select(m => m.Value)
                                  .ToArray();

            return attributes.Select(a =>
            {
                var attr = a.Split('=');
                return (attr[0], Regex.Match(attr[1], @"[^""']+").Value);
            });
        }

        public static string GetTagName(this string xmlTag) => Regex.Match(xmlTag, @"[\w-\:]+").Value;

        public static XmlTokenType GetTokenType(this string xmlToken)
        {
            if (IsDeclaration(xmlToken)) return XmlTokenType.Declaration;
            if (IsEnclosedTag(xmlToken)) return XmlTokenType.EnclosedTag;
            if (IsStartTag(xmlToken)) return XmlTokenType.StartTag;
            if (IsClosingTag(xmlToken)) return XmlTokenType.ClosingTag;
            if (IsWhitespace(xmlToken)) return XmlTokenType.Whitespace;

            return XmlTokenType.Value;
        }

        public static bool IsDeclaration(string token) => Regex.IsMatch(token, XmlDeclarationRegexp);

        public static bool IsEnclosedTag(string token) => Regex.IsMatch(token, XmlEnclosedTagRegex);

        public static bool IsStartTag(string token) => Regex.IsMatch(token, XmlStartTagRegex);

        public static bool IsClosingTag(string token) => Regex.IsMatch(token, XmlClosingTagRegex);

        public static bool IsWhitespace(string token) => Regex.IsMatch(token, WhitespaceRegex);
    }
}
