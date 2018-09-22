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

		public bool HasTokens(ArraySegment<byte> buffer)
		{
			if (buffer.Array.Length == 0)
			{
				return false;
			}

			int ltIndex = IndexOf(buffer, '<', Position);

			return ltIndex >= 0 && IndexOf(buffer, '>', ltIndex) >= 0;
		}

		public bool Read(ArraySegment<byte> buffer)
		{
			int ltIndex = IndexOf(buffer, '<', Position);
			int gtIndex;

			if (ltIndex < 0 || (gtIndex = IndexOf(buffer, '>', ltIndex)) < 0)
			{
				TokenName = null;
				TokenType = XmlTokenType.None;
				return false;
			}

			if (buffer.Array[ltIndex + 1] == '?' && buffer.Array[gtIndex - 1] == '?')
			{
				TokenName = string.Empty;
				TokenType = XmlTokenType.Declaration;
				Position = gtIndex;
				return true;
			}

			if (ltIndex - Position > 1 && !IsNextWithoutSpaces(buffer, Position, ltIndex))
			{
				UpdateNameAndType(buffer, Position + 1, ltIndex - Position - 1, XmlTokenType.Value);
				Position = ltIndex;
				return true;
			}

			int slashIndex = LastIndexOf(buffer, '/', ltIndex, gtIndex);

			if (IsStartTag(buffer, slashIndex, ltIndex, gtIndex))
			{
				UpdateNameAndType(buffer, ltIndex + 1, gtIndex - ltIndex - 1, XmlTokenType.StartTag);
			}
			else if (IsNextWithoutSpaces(buffer, slashIndex, gtIndex))
			{
				UpdateNameAndType(buffer, ltIndex + 1, slashIndex - ltIndex - 1, XmlTokenType.EnclosedTag);
			}
			else
			{
				UpdateNameAndType(buffer, slashIndex + 1, gtIndex - slashIndex - 1, XmlTokenType.ClosingTag);
			}

			Position = gtIndex;
			return true;
		}

		private bool IsNextWithoutSpaces(ArraySegment<byte> buffer, int startIndex, int endIndex)
		{
			for (int i = startIndex + 1; i < endIndex; i++)
			{
				if (!char.IsWhiteSpace((char)buffer.Array[i]))
				{
					return false;
				}
			}

			return true;
		}

		private bool IsStartTag(ArraySegment<byte> buffer, int slashIndex, int ltIndex, int gtIndex)
		{
			return slashIndex == -1
				|| (!IsNextWithoutSpaces(buffer, slashIndex, gtIndex)
				&& !IsNextWithoutSpaces(buffer, ltIndex, slashIndex));
		}

		private void UpdateNameAndType(ArraySegment<byte> buffer, int startIndex, int count, XmlTokenType tokenType)
		{
			var str = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + startIndex, count);

			TokenName = str.Trim().Split(' ')[0];
			TokenType = tokenType;
		}

		public void Reset()
		{
			Position = 0;
		}

		private int IndexOf(ArraySegment<byte> buffer, char c, int startIndex)
		{
			for (int i = startIndex; i < buffer.Count; i++)
			{
				if (buffer.Array[i] == c)
				{
					return i;
				}
			}

			return -1;
		}

		private int LastIndexOf(ArraySegment<byte> buffer, char c, int startIndex, int endIndex)
		{
			for (int i = endIndex; i >= startIndex; i--)
			{
				if (buffer.Array[i] == c)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
