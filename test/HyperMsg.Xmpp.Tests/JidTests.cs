using System;
using System.Text;
using Xunit;

namespace HyperMsg.Xmpp
{
	public class JidTests
	{
		private static readonly string userName = "Fact-user";
		private static readonly string domain = "www.domain.com";
		private static readonly string resource = "res";
		private static readonly string fullJid = userName + "@" + domain + "/" + resource;
		private static readonly int MaxJidPartLength = 1023;

		private static readonly string[] incorrectDomains = { "d@main", "domain/" };

		[InlineData("us/r")]
		[Theory(DisplayName = "Constructor throws exception if invalid user name provided")]
		public void Constructor_Throws_Exception_If_Invalid_User_Name_Provided(string userName)
		{
			Assert.Throws<ArgumentException>(() => new Jid(userName, domain, ""));
		}

		[Fact(DisplayName = "Constructor throws exception if user name more than 1023 characters")]
		public void Constructor_Throws_Exception_If_User_Name_More_Than_1023_Characters()
		{
			string userPart = GenerateJidPart(MaxJidPartLength + 1);

			Assert.Throws<ArgumentException>(() => new Jid(userPart, domain, null));
		}

		[InlineData("d@main")]
		[InlineData("domain/")]
		[Theory(DisplayName = "Constructor throws exception if domain name more then 1023 characters")]
		public void Constructor_Throws_Exception_If_Domain_Name_More_Then_1023_Characters(string domain)
		{
			Assert.Throws<ArgumentException>(() => new Jid("", domain, ""));
		}

		[Fact(DisplayName = "Constructor throws exxception if domain length more than 1023 characters")]
		public void Constructor_DomainLengthMoreThan1023_ThrowsException()
		{
			string domainPart = GenerateJidPart(MaxJidPartLength + 1);

			Assert.Throws<ArgumentException>(() => new Jid(null, domainPart, null));
		}

		[Fact(DisplayName = "Constructor throws eception if resource length more than 1023 characters")]
		public void Constructor_ResourceLengthMoreThan1023_ThrowsException()
		{
			string resourcePart = GenerateJidPart(MaxJidPartLength + 1);

			Assert.Throws<ArgumentException>(() => new Jid(null, domain, resourcePart));
		}

		private string GenerateJidPart(int length)
		{
			StringBuilder jidPart = new StringBuilder(length);
			Random random = new Random(DateTime.Now.Millisecond);
			for (int i = 0; i < length; i++)
			{
				jidPart.Append((char)random.Next('A', 'Z'));
			}
			return jidPart.ToString();
		}

		[Fact(DisplayName = "Parse correctly parses JID string")]
		public void Parse_CorrectJidString_ReturnsCorrectJidObject()
		{
			Jid result = Jid.Parse(fullJid);

			Assert.Equal(userName, (result.User));
			Assert.Equal(domain, (result.Domain));
			Assert.Equal(resource, (result.Resource));
		}

		[Fact(DisplayName = "ToString returns correct JID string")]
		public void ToString_UserNameAndResourceNotEmpty_ReturnsCorrectJidString()
		{
			Jid jid = new Jid(userName, domain, resource);
			string expectedJid = fullJid;

			string actualJid = jid.ToString();

			Assert.Equal(expectedJid, (actualJid));
		}

		[Fact(DisplayName = "Parse parses JID string without resource part")]
		public void Parse_NotEmptyUserNameEmptyResource_ReturnsCorrectJidObject()
		{
			string jidString = userName + "@" + domain;

			Jid jid = Jid.Parse(jidString);

			Assert.Equal(userName, (jid.User));
			Assert.Equal(domain, (jid.Domain));
			Assert.Empty(jid.Resource);
		}

		[Fact(DisplayName = "Parse parses JID string with domain only")]
		public void Parse_UserNameAndResourceEmpty_ReturnsCorrectJidObject()
		{
			Jid jid = Jid.Parse(domain);

			Assert.Equal(domain, (jid.Domain));
			Assert.Empty(jid.User);
			Assert.Empty(jid.Resource);
		}

		[Fact(DisplayName = "Equals returns True for equal JIDs")]
		public void Equals_EqualJids_ReturnsTrue()
		{
			Jid jid1 = new Jid("amy", "lee", "evanescence");
			Jid jid2 = new Jid("amy", "lee", "evanescence");

			Assert.True(jid1.Equals(jid2));
		}

		[Fact(DisplayName = "Equals returns False for inequal JIDs")]
		public void Can_Correctly_Compare_Inequal_Jids()
		{
			Jid jid1 = new Jid("amy", "lee", "evanescence");
			Jid jid2 = new Jid("bruce", "lee", "JetQuinDo");

			Assert.False(jid1.Equals(jid2));
		}
	}
}
