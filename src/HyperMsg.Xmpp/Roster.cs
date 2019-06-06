namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Contains static factory methods that used for creating 'iq' stanzas related to roster operations.
    /// </summary>
    public static class Roster
    {
        /// <summary>
        /// XML namespace for roster elements (usually exists in query element)
        /// </summary>
        public static readonly string RosterXmlns = "jabber:iq:roster";

        /// <summary>
        /// Returns stanza that is roster request.
        /// </summary>
        /// <returns>
        /// 'iq' stanza for roster request.
        /// </returns>
        public static XmlElement Get() => Iq.Get().Children(Query().Xmlns(RosterXmlns));

        /// <summary>
        /// Returns stanza that is roster request.
        /// </summary>
        /// <param name="from">
        /// Value of 'from' attribute.
        /// </param>
        /// <returns>
        /// 'iq' stanza for roster request.
        /// </returns>
        public static XmlElement Get(string from) => Get().From(from);

        /// <summary>
        /// Returns stanza that is roster update request (used to add or update roster item).
        /// </summary>
        /// <param name="jid">
        /// Item JID.
        /// </param>
        /// <param name="name">
        /// Item name.
        /// </param>
        /// <param name="groups">
        /// Groups which item belongs to.
        /// </param>
        /// <returns>
        /// 'iq' stanza for roster update requests.
        /// </returns>
        public static XmlElement Set(Jid jid, string name, params string[] groups) => Iq.Set().Children(Query().Children(Item(jid, name, groups)));

        /// <summary>
        /// Returns stanza that is roster delete request (used to delete roster item).
        /// </summary>
        /// <param name="jid">
        /// Item JID.
        /// </param>
        /// <returns>
        /// 'iq' stanza that is roster delete request.
        /// </returns>
        public static XmlElement Remove(Jid jid) => Iq.Set().Children(Query().Children(Item(jid).Attribute("subscription", "remove")));

        private static XmlElement Query() => new XmlElement("query").Xmlns(RosterXmlns);

        private static XmlElement Item(Jid jid, string name = null)
        {
            var item = new XmlElement("item").Attribute("jid", jid);

            if (!string.IsNullOrEmpty(name))
            {
                item["name"] = name;
            }

            return item;
        }

        private static XmlElement Item(Jid jid, string name, string[] groups)
        {
            var item = Item(jid, name);

            for (int i = 0; i < groups.Length; i++)
            {
                item.Children.Add(new XmlElement("group")
                    .Value(groups[i]));
            }

            return item;
        }
    }
}
