using Xunit;

namespace HyperMsg.Xmpp.Xml
{
    public class StreamHeaderTests
    {
        [Fact]
        public void Client_Returns_Correct_Client_Stream_Header()
        {
            var header = StreamHeader.Client();

            Assert.Equal("stream:stream", header.Name);
            Assert.Equal("1.0", header["version"]);
            Assert.Equal(header["xmlns"], XmppNamespaces.JabberClient);
            Assert.Equal(header["xmlns:stream"], XmppNamespaces.Streams);
        }

        [Fact]
        public void Server_Returns_Correct_Server_Stream_Header()
        {
            var header = StreamHeader.Server();

            Assert.Equal("stream:stream", header.Name);
            Assert.Equal("1.0", header["version"]);
            Assert.Equal(header["xmlns"], XmppNamespaces.JabberServer);
            Assert.Equal(header["xmlns:stream"], XmppNamespaces.Streams);
        }
    }
}
