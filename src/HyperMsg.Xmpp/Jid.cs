using System;
using System.Text;
using System.Text.RegularExpressions;

namespace HyperMsg.Xmpp
{
	public class Jid
	{
		public const int MaxJidPartByteLength = 1023;

		private static readonly char[] InvalidUserPartChars = { '@', '/' };
		private static readonly char[] InvalidDomainPartChars = { '@', '/' };
		private static readonly char[] InvalidResourcePartChars = { '/' };
		private string user;
		private string domain;
		private string resource;

		public Jid(string bareJid)
		{
			if (!IsValid(bareJid)) throw new ArgumentException("Invalid format for Jid");

			int domSeparatorPos = bareJid.IndexOf('@');
			int resSeparatorPos = bareJid.IndexOf('/');
			int domainStrLen = resSeparatorPos < 0 ? bareJid.Length - domSeparatorPos - 1 : resSeparatorPos - domSeparatorPos - 1;

			User = domSeparatorPos < 0 ? "" : bareJid.Substring(0, domSeparatorPos);
			Domain = bareJid.Substring(domSeparatorPos + 1, domainStrLen);
			Resource = resSeparatorPos < 0 ? "" : bareJid.Substring(resSeparatorPos + 1, bareJid.Length - resSeparatorPos - 1);
		}

		public Jid(string user, string domain) : this(user, domain, null)
		{ }

		public Jid(string user, string domain, string resource)
		{
			User = user;
			Domain = domain;
			Resource = resource;
		}

		public string User
		{
			get { return user; }
			set
			{
				if (!string.IsNullOrEmpty(value)) ValidateUserFormat(value);
				user = value;
			}
		}
		public string Domain
		{
			get { return domain; }
			set
			{
				ValidateDomainFormat(value);
				domain = value;
			}
		}
		public string Resource
		{
			get { return resource; }
			set
			{
				if (!string.IsNullOrEmpty(value)) ValidateResourceFormat(value);
				resource = value;
			}
		}

		private void ValidateUserFormat(string user)
		{
			ValidateUserPartCharacters(user);
			if (!IsValidPartLength(user)) throw new ArgumentException();
		}

		private void ValidateUserPartCharacters(string user)
		{
			ValidateCharacters(user, InvalidUserPartChars, "");
		}

		private void ValidateDomainFormat(string domain)
		{
			ValidateDomainPartCharacters(domain);
			if (!IsValidPartLength(domain)) throw new ArgumentException();
		}

		private void ValidateDomainPartCharacters(string domain)
		{
			ValidateCharacters(domain, InvalidDomainPartChars, "");
		}

		public void ValidateResourceFormat(string resource)
		{
			ValidateResourcePartCharacters(resource);
			if (!IsValidPartLength(resource)) throw new ArgumentException();
		}

		private void ValidateResourcePartCharacters(string resource)
		{
			ValidateCharacters(resource, InvalidResourcePartChars, "");
		}

		private void ValidateCharacters(string jidPart, char[] invalidCharacters, string exceptionMessage)
		{
			int invalidCharIndex = jidPart.IndexOfAny(invalidCharacters);
			if (invalidCharIndex > -1) throw new ArgumentException(string.Format(exceptionMessage, jidPart[invalidCharIndex]));
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(User))
			{
				sb.Append(User);
				sb.Append('@');
			}
			sb.Append(Domain);
			if (!string.IsNullOrEmpty(Resource))
			{
				sb.Append('/');
				sb.Append(Resource);
			}
			return sb.ToString();
		}

		public static Jid Parse(string bareJid)
		{
			return new Jid(bareJid);
		}

		public static implicit operator Jid(string bareJid)
		{
			return new Jid(bareJid);
		}

		public static implicit operator string(Jid jid)
		{
			return jid.ToString();
		}

		private static bool IsValidPartLength(string jidPart)
		{
			return Encoding.UTF8.GetByteCount(jidPart) <= MaxJidPartByteLength;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is Jid)) return false;
			Jid jid = (Jid)obj;
			return Equals(jid);
		}

		public bool Equals(Jid jid)
		{
			return User == jid.User &&
				   Domain == jid.Domain &&
				   Resource == jid.Resource;
		}

		public static bool IsValid(string bareJid)
		{
			return Regex.IsMatch(bareJid, @"(\S+@)?\w+(\.\w{3})?(/\w*)?");
		}
	}
}
