using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HyperMsg.Xmpp.Serialization
{
    public class XmlLexer
    {
        private static readonly string XmlDeclarationRegexp = @"\<\?xml[^\<\>\\]*\?\>";
        private static readonly string XmlEnclosedTagRegex = @"\<[\w-\:]+[^\<\>\\]*\/\>";
        private static readonly string XmlStartTagRegex = @"\<[\w-\:]+[^\<\>]*[^\\]\>";
        private static readonly string XmlClosingTagRegex = @"\<\/\s*[\w-\:]+\>";
        private static readonly string XmlValueRegex = @"[^\<\>\\]+";
        private static readonly string WhitespaceRegex = @"^[\s\t]+";
        private static readonly string GenericXmlElementRegex = string.Format(@"({0})|({1})|({2})|({3})|({4})", XmlDeclarationRegexp, XmlStartTagRegex, XmlClosingTagRegex, XmlEnclosedTagRegex, XmlValueRegex);

        private static readonly string XmlAttributeRegex = @"[\w-\:]+=('|"")[\S]*\1";

        public XmlToken[] GetTokens(string xmlFragment, bool ignoreWhitespaces = true)
        {
            MatchCollection matches = Regex.Matches(xmlFragment, GenericXmlElementRegex);
            List<XmlToken> tokens = new List<XmlToken>(matches.Count);

            for (int i = 0; i < matches.Count; i++)
            {
                string value = matches[i].Value;

                if (IsWhitespace(value) && ignoreWhitespaces)
                {
                    continue;
                }

                tokens.Add(GetToken(value));
            }

            return tokens.ToArray();
        }

        public XmlToken GetToken(string value)
        {
            var tokenType = GetTokenType(value);
            XmlToken token = new XmlToken(new System.Buffers.ReadOnlySequence<byte>(), XmlTokenType.ClosingTag);

            //if (options.ParseTagName && IsTag(token))
            //{
            //    token.Name = GetTagName(token);
            //}

            return token;
        }

        public static IEnumerable<Tuple<string, string>> GetTagAttributes(XmlToken token)
        {
            //var attributes = Regex.Matches(token.Value, XmlAttributeRegex)
            //                      .Cast<Match>()
            //                      .Select(m => m.Value)
            //                      .ToArray();

            //return attributes.Select(a =>
            //{
            //    var attr = a.Split('=');
            //    return new Tuple<string, string>(attr[0], Regex.Match(attr[1], @"[^""']+").Value);
            //});

            return null;
        }

        public static string GetTagName(XmlToken token)
        {
            return "";// Regex.Match(token.Value, @"[\w-\:]+").Value;
        }

        public static XmlTokenType GetTokenType(string token)
        {
            if (IsDeclaration(token)) return XmlTokenType.Declaration;
            if (IsEnclosedTag(token)) return XmlTokenType.EnclosedTag;
            if (IsStartTag(token)) return XmlTokenType.StartTag;
            if (IsClosingTag(token)) return XmlTokenType.ClosingTag;
            if (IsWhitespace(token)) return XmlTokenType.Whitespace;
            return XmlTokenType.Value;
        }

        public static bool IsTag(XmlToken token)
        {
            return token.Type == XmlTokenType.StartTag ||
                   token.Type == XmlTokenType.ClosingTag ||
                   token.Type == XmlTokenType.EnclosedTag;
        }

        public static bool IsDeclaration(string token)
        {
            return Regex.IsMatch(token, XmlDeclarationRegexp);
        }

        public static bool IsEnclosedTag(string token)
        {
            return Regex.IsMatch(token, XmlEnclosedTagRegex);
        }

        public static bool IsStartTag(string token)
        {
            return Regex.IsMatch(token, XmlStartTagRegex);
        }

        public static bool IsClosingTag(string token)
        {
            return Regex.IsMatch(token, XmlClosingTagRegex);
        }

        public static bool IsWhitespace(string token)
        {
            return Regex.IsMatch(token, WhitespaceRegex);
        }
    }

    public class XmlLexerOptions
    {
        public bool ParseTagName { get; set; }
        public bool IgnoreWhitespaces { get; set; }
    }

    

    public enum XmlTokenType
    {
        None,
        Declaration,
        EnclosedTag,
        StartTag,
        ClosingTag,
        Value,
        Whitespace
    }
}
