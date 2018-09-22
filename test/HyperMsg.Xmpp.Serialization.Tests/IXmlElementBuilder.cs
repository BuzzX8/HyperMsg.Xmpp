using System.Collections.Generic;

namespace HyperMsg.Xmpp.Serialization.Tests
{
    public interface IXmlElementBuilder
    {
        XmlElement Build(IList<XmlToken> tokens);
    }
}
