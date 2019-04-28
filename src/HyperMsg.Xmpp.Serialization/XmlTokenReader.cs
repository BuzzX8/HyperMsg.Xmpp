using System;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
	public class XmlTokenReader
	{
		public int Position { get; internal set; }

		public XmlTokenType TokenType
		{
			get; private set;
		}

		public string TokenName
		{
			get; private set;
		}

		public bool HasTokens(Memory<byte> buffer)
		{
			if (buffer.Length == 0)
			{
				return false;
			}

            int ltIndex = IndexOf(buffer.Span, '<', Position);

            return ltIndex >= 0 && IndexOf(buffer.Span, '>', ltIndex) >= 0;
		}

		public bool Read(Memory<byte> buffer)
		{
            var span = buffer.Span;
			int ltIndex = IndexOf(span, '<', Position);
			int gtIndex;

			if (ltIndex < 0 || (gtIndex = IndexOf(span, '>', ltIndex)) < 0)
			{
				TokenName = null;
				TokenType = XmlTokenType.None;
				return false;
			}

			if (span[ltIndex + 1] == '?' && span[gtIndex - 1] == '?')
			{
				TokenName = string.Empty;
				TokenType = XmlTokenType.Declaration;
				Position = gtIndex;
				return true;
			}

			if (ltIndex - Position > 1 && !IsNextWithoutSpaces(span, Position, ltIndex))
			{
				UpdateNameAndType(span, Position + 1, ltIndex - Position - 1, XmlTokenType.Value);
				Position = ltIndex;
				return true;
			}

			int slashIndex = LastIndexOf(span, '/', ltIndex, gtIndex);

			if (IsStartTag(span, slashIndex, ltIndex, gtIndex))
			{
				UpdateNameAndType(span, ltIndex + 1, gtIndex - ltIndex - 1, XmlTokenType.StartTag);
			}
			else if (IsNextWithoutSpaces(span, slashIndex, gtIndex))
			{
				UpdateNameAndType(span, ltIndex + 1, slashIndex - ltIndex - 1, XmlTokenType.EnclosedTag);
			}
			else
			{
				UpdateNameAndType(span, slashIndex + 1, gtIndex - slashIndex - 1, XmlTokenType.ClosingTag);
			}

			Position = gtIndex;
			return true;
		}

		private bool IsNextWithoutSpaces(Span<byte> buffer, int startIndex, int endIndex)
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

		private bool IsStartTag(Span<byte> buffer, int slashIndex, int ltIndex, int gtIndex)
		{
			return slashIndex == -1
				|| (!IsNextWithoutSpaces(buffer, slashIndex, gtIndex)
				&& !IsNextWithoutSpaces(buffer, ltIndex, slashIndex));
		}

		private void UpdateNameAndType(Span<byte> buffer, int startIndex, int count, XmlTokenType tokenType)
		{
			var str = Encoding.UTF8.GetString(buffer.ToArray(), startIndex, count);

			TokenName = str.Trim().Split(' ')[0];
			TokenType = tokenType;
		}

		public void Reset()
		{
			Position = 0;
		}

		private int IndexOf(Span<byte> buffer, char c, int startIndex)
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

		private int LastIndexOf(Span<byte> buffer, char c, int startIndex, int endIndex)
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
