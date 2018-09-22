using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg.Xmpp.Tests
{
    public class PresenceTests
    {
        public static IEnumerable<object[]> GetCreateMethodTestCase()
        {
            yield return CreateMethodTestCase(Presence.Available, null);
            yield return CreateMethodTestCase(Presence.Unavailable, Presence.Type.Unavailable);
            yield return CreateMethodTestCase(Presence.Subscribe, Presence.Type.Subscribe);
            yield return CreateMethodTestCase(Presence.Subscribed, Presence.Type.Subscribed);
            yield return CreateMethodTestCase(Presence.Unsubscribe, Presence.Type.Unsubscribe);
            yield return CreateMethodTestCase(Presence.Unsubscribed, Presence.Type.Unsubscribed);
            yield return CreateMethodTestCase(Presence.Error, Presence.Type.Error);
            yield return CreateMethodTestCase(Presence.Probe, Presence.Type.Probe);
        }

        private static object[] CreateMethodTestCase(Func<XmlElement> method, string expectedType)
        {
            return new object[] { method, expectedType };
        }

        [Theory]
        [MemberData(nameof(GetCreateMethodTestCase))]
        public void VerifyCreationMethod(Func<XmlElement> method, string expectedType)
        {
            var stanza = method();
            Assert.True(stanza.IsPresence());
            Assert.Equal(stanza.Type(), expectedType);
        }

        [Fact]
        public void Status_Returns_Element_With_Correct_Status_Child_Element()
        {
            var value = Guid.NewGuid().ToString();
            var stanza = Presence.New().Status(value);
            Assert.Equal(stanza.Child("status").Value, value);
        }

        public static IEnumerable<object[]> GetShowTestCases()
        {
            yield return ShowTestCase(Presence.ShowAway, Presence.ShowStatus.Away);
            yield return ShowTestCase(Presence.ShowChat, Presence.ShowStatus.Chat);
            yield return ShowTestCase(Presence.ShowDoNotDisturb, Presence.ShowStatus.DoNotDisturb);
            yield return ShowTestCase(Presence.ShowExtendedAway, Presence.ShowStatus.ExtendedAway);
        }

        private static object[] ShowTestCase(Func<XmlElement, XmlElement> showFunc, string expectedValue)
        {
            return new object[] { showFunc, expectedValue };
        }

        [Theory]
        [MemberData(nameof(GetShowTestCases))]
        public void VerifyShowMethod(Func<XmlElement, XmlElement> showFunc, string expectedValue)
        {
            var stanza = showFunc(Presence.New());
            Assert.Equal(stanza.Child("show").Value, expectedValue);
        }
    }
}
