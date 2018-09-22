using System.Linq;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Provides extension methods for fluent manipulation of XmlElement
    /// </summary>
    public static class XmlElementExtensions
    {
        /// <summary>
        /// Updates attribute value for XmlElement.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be updated.
        /// </param>
        /// <param name="name">
        /// Attribute name.
        /// </param>
        /// <param name="value">
        /// Attribute value.
        /// </param>
        /// <returns>
        /// Updated XmlElement.
        /// </returns>
        public static XmlElement Attribute(this XmlElement element, string name, object value)
        {
            element.SetAttributeValue(name, value);

            return element;
        }

        /// <summary>
        /// Updates attribute value for XmlElement.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be updated.
        /// </param>
        /// <param name="name">
        /// Attribute name.
        /// </param>
        /// <param name="value">
        /// Attribute value.
        /// </param>
        /// <returns>
        /// Updated XmlElement.
        /// </returns>
        public static XmlElement Attribute(this XmlElement element, string name, string value)
        {
            element.SetAttributeValue(name, value);

            return element;
        }

        /// <summary>
        /// Returns child element with specified name or null if no child with such name.
        /// </summary>
        /// <param name="element">
        /// Element which child elements should be scanned.
        /// </param>
        /// <param name="childName">
        /// Name of child element.
        /// </param>
        /// <returns>
        /// Child XML element with specified name of null if no elements with such name.
        /// </returns>
        public static XmlElement Child(this XmlElement element, string childName)
        {
            return element.Children.FirstOrDefault(e => e.Name == childName);
        }

        /// <summary>
        /// Adds a set of xml elements as child elements for XmlElement.
        /// </summary>
        /// <param name="element">
        /// Element to which add child elements.
        /// </param>
        /// <param name="children">
        /// Elements which should be added as child elements.
        /// </param>
        /// <returns>
        /// Updated XmlElement.
        /// </returns>
        public static XmlElement Children(this XmlElement element, params XmlElement[] children)
        {
            foreach (var child in children)
            {
                element.Children.Add(child);
            }

            return element;
        }

        /// <summary>
        /// Returns true if element contains child with specified name, otherwise false.
        /// </summary>
        /// <param name="element">
        /// Element which will be checked for existence of child element.
        /// </param>
        /// <param name="childName">
        /// Name of child element.
        /// </param>
        /// <returns>
        /// true if current element has child with specified name, otherwise false.
        /// </returns>
        public static bool HasChild(this XmlElement element, string childName)
        {
            return element.Children.Any(e => e.Name == childName);
        }

        /// <summary>
        /// Assignes <paramref name="value"/> to Value property of XmlElement.
        /// </summary>
        /// <param name="element">
        /// Element which value should be updated.
        /// </param>
        /// <param name="value">
        /// New value of XmlElement.
        /// </param>
        /// <returns>
        /// Element with updated value.
        /// </returns>
        public static XmlElement Value(this XmlElement element, string value)
        {
            element.Value = value;

            return element;
        }

        /// <summary>
        /// Returns value of xmlns attribute or null if element does not contains such attribute.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be readed
        /// </param>
        /// <returns>
        /// Value of xmlns attribute or null if no such attribute exists.
        /// </returns>
        public static string Xmlns(this XmlElement element)
        {
            return element.GetAttributeValue("xmlns");
        }

        /// <summary>
        /// Updates value of xmlns attribute.
        /// </summary>
        /// <param name="element">
        /// Element which xmlns attribute should be updated.
        /// </param>
        /// <param name="xmlns">
        /// Value of xmlns attribute.
        /// </param>
        /// <returns>
        /// Updated XmlElement.
        /// </returns>
        public static XmlElement Xmlns(this XmlElement element, string xmlns)
        {
            element.SetAttributeValue("xmlns", xmlns);

            return element;
        }

        /// <summary>
        /// Returns value of xml:lang attribute or null if element does not contains such attribute.
        /// </summary>
        /// <param name="element">
        /// Element which attribute value should be readed
        /// </param>
        /// <returns>
        /// Value of xml:lang attribute or null if no such attribute exists.
        /// </returns>
        public static string XmlLang(this XmlElement element)
        {
            return element.GetAttributeValue("xml:lang");
        }

        /// <summary>
        /// Updates value of xml:lang attribute.
        /// </summary>
        /// <param name="element">
        /// Element which xmlns attribute should be updated.
        /// </param>
        /// <param name="xmlns">
        /// Value of xml:lang attribute.
        /// </param>
        /// <returns>
        /// Updated XmlElement.
        /// </returns>
        public static XmlElement XmlLang(this XmlElement element, string lang)
        {
            element.SetAttributeValue("xml:lang", lang);

            return element;
        }
    }
}
