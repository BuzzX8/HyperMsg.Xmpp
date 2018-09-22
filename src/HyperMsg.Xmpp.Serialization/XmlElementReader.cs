using System;
using System.Text;

namespace HyperMsg.Xmpp.Serialization
{
	public class XmlElementReader
	{
		private XmlTokenReader tokenReader;

		public XmlElementReader()
		{
			tokenReader = new XmlTokenReader();
		}

		internal int Position { get; set; }

		public bool HasElement(ArraySegment<byte> buffer)
		{
			int previousPosition = tokenReader.Position;
			tokenReader.Position = Position;

			try
			{
				return TryReadElement(buffer);
			}
			finally
			{
				tokenReader.Position = previousPosition;
			}
		}

		private bool TryReadElement(ArraySegment<byte> buffer)
		{
			if (!tokenReader.Read(buffer))
			{
				return false;
			}

			if (tokenReader.TokenType == XmlTokenType.Declaration)
			{
				tokenReader.Read(buffer);
			}

			if (tokenReader.TokenType == XmlTokenType.EnclosedTag)
			{
				return true;
			}

			if (tokenReader.TokenType == XmlTokenType.ClosingTag)
			{
				return tokenReader.TokenName == "stream:stream";
			}

			if (tokenReader.TokenType == XmlTokenType.StartTag)
			{
				if (tokenReader.TokenName == "stream:stream")
				{
					return true;
				}

				return IsMultiTagElement(buffer);
			}

			return false;
		}

		private bool IsMultiTagElement(ArraySegment<byte> buffer)
		{
			string name = tokenReader.TokenName;

			while (tokenReader.Read(buffer))
			{
				if (IsClosingTag(name))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsClosingTag(string tagName)
		{
			return tokenReader.TokenType == XmlTokenType.ClosingTag
				&& tokenReader.TokenName == tagName;
		}

		public string ReadElement(ArraySegment<byte> buffer)
		{
			if (!TryReadElement(buffer))
			{
				throw new Exception();
			}

			var xml = Encoding.UTF8.GetString(buffer.Array, Position, tokenReader.Position + 1);
			Position = tokenReader.Position;
			return xml;
		}

		public void Reset()
		{
			tokenReader.Reset();
			Position = 0;
		}
	}
}