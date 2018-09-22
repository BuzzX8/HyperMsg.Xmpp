using System.Collections.Generic;

namespace HyperMsg.Xmpp.Serialization.Tests
{
	public class XmlElementBuilder : IXmlElementBuilder
	{
		public XmlElement Build(IList<XmlToken> tokens)
		{
			XmlElement element = null;
			Stack<XmlElement> parents = null;

			foreach (XmlToken token in tokens)
			{
				if (token.Type == XmlTokenType.Whitespace)
				{
					continue;
				}

				if (token.Type == XmlTokenType.EnclosedTag)
				{
					var encElement = CreateElement(token);

					if (element != null)
					{
						element.Children.Add(encElement);
					}
					else
					{
						element = encElement;
					}

					continue;
				}

				if (token.Type == XmlTokenType.StartTag)
				{
					if (element != null)
					{
						if (parents == null)
						{
							parents = new Stack<XmlElement>();
						}

						var child = CreateElement(token);
						element.Children.Add(child);
						parents.Push(element);
						element = child;
					}
					else
					{
						element = CreateElement(token);
					}
					continue;
				}

				if (token.Type == XmlTokenType.Value)
				{
					element.Value = token.Value;
					continue;
				}

				if (token.Type == XmlTokenType.ClosingTag)
				{
					if (parents != null && parents.Count > 0)
					{
						element = parents.Pop();
					}

					if (element == null)
					{
						element = new XmlElement('/' + token.TagName);
					}
				}
			}

			return element;
		}

		private XmlElement CreateElement(XmlToken token)
		{
			var element = new XmlElement(token.TagName);
			AddAttributes(element, token);
			return element;
		}

		private void AddAttributes(XmlElement element, XmlToken token)
		{
			foreach (var attr in XmlLexer.GetTagAttributes(token))
			{
				element.SetAttributeValue(attr.Item1, attr.Item2);
			}
		}
	}
}