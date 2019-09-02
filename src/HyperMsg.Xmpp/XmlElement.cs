using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperMsg.Xmpp
{
    /// <summary>
    /// Represents an Xml element that is a basic unit for XMPP stream message.
    /// </summary>
    public class XmlElement : IEquatable<XmlElement>
    {
        private Dictionary<string, string> attributes;
        private List<XmlElement> children;

        /// <summary>
        /// Initializes new instance of XmlElement with specified name.
        /// </summary>
        /// <param name="name">
        /// Name of XML element.
        /// </param>
        public XmlElement(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Initializes new instance of XmlElement with specified name and child elements.
        /// </summary>
        /// <param name="name">
        /// Name of XML element.
        /// </param>
        /// <param name="children">
        /// Child elements.
        /// </param>
        public XmlElement(string name, params XmlElement[] children) : this(name)
        {
            this.children = new List<XmlElement>(children);
        }

        /// <summary>
        /// Returns name of current XmlElement
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets value of attribute with specified name.
        /// </summary>
        /// <param name="name">
        /// Name of attribute.
        /// </param>
        /// <returns>
        /// Returns value of attribute with specified name of returns <b>null</b> if no parameter
        /// with such name.
        /// </returns>
        public string this[string name]
        {
            get => GetAttributeValue(name);
            set => SetAttributeValue(name, value);
        }

        private Dictionary<string, string> Attributes => attributes ?? (attributes = new Dictionary<string, string>());

        /// <summary>
        /// Returns collection of child elements.
        /// </summary>
        public ICollection<XmlElement> Children => children ?? (children = new List<XmlElement>());

        /// <summary>
        /// Returns value of current XmlElement
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns <b>true</b> if element contains at least one attribute, otherwise returns <b>false</b>.
        /// </summary>
        public bool HasAttributes => attributes != null && attributes.Count > 0;

        /// <summary>
        /// Returns <b>true</b> if element contains at least one child element, otherwise returns <b>false</b>.
        /// </summary>
        public bool HasChildren => children != null && children.Count > 0;

        /// <summary>
        /// Returns <b>true</b> if current XmlElement contains attribute with specified name,
        /// otherwise returns <b>false</b>.
        /// </summary>
        /// <param name="name">
        /// Name of attribute.
        /// </param>
        /// <returns>
        /// <b>true</b> if attribute with provided name exists, otherwise <b>false</b>.
        /// </returns>
        public bool HasAttribute(string name) => attributes != null && attributes.ContainsKey(name);

        /// <summary>
        /// Returns value of attribute with spcfied name, if attribute with such name
        /// does not exists returns <b>null</b>.
        /// </summary>
        /// <param name="name">
        /// Attribute name.
        /// </param>
        /// <returns>
        /// Value of attribute with specified name of <b>null</b> if no such attribute exists.
        /// </returns>
        public string GetAttributeValue(string name)
        {
            if (attributes == null || !attributes.ContainsKey(name))
            {
                return null;
            }

            return Attributes[name];
        }

        /// <summary>
        /// Updates attribute with specified name.
        /// </summary>
        /// <param name="name">
        /// Attribute name.
        /// </param>
        /// <param name="value">
        /// Atttribute value.
        /// </param>
        public void SetAttributeValue(string name, object value)
        {
            if (value != null)
            {
                Attributes[name] = value.ToString();
            }
            else
            {
                if (Attributes.ContainsKey(name))
                {
                    Attributes.Remove(name);
                }
            }
        }

        /// <summary>
        /// Updates attribute with specified name.
        /// </summary>
        /// <param name="name">
        /// Attribute name.
        /// </param>
        /// <param name="value">
        /// Atttribute value.
        /// </param>
        public void SetAttributeValue(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Attributes[name] = value;
            }
            else
            {
                if (Attributes.ContainsKey(name))
                {
                    Attributes.Remove(name);
                }
            }
        }

        /// <summary>
        /// Iterates over all attributes and invoke <paramref name="action"/> for each of them.
        /// </summary>
        /// <param name="action">
        /// Action that is invokes for each attribute.
        /// </param>
        public void ForEachAttribute(Action<string, string> action)
        {
            foreach (var attribute in Attributes)
            {
                action(attribute.Key, attribute.Value);
            }
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();

            if (attributes != null)
            {
                hash ^= attributes.Aggregate(0, (a, kvp) => a ^= kvp.Key.GetHashCode() ^ kvp.Value.GetHashCode());
            }

            if (children != null)
            {
                hash ^= children.Aggregate(0, (a, c) => a ^= c.GetHashCode());
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is XmlElement element))
            {
                return false;
            }

            return Equals(element);
        }

        public bool Equals(XmlElement element)
        {
            if (!AreChildrenEqual(element))
            {
                return false;
            }

            if (HasAttributes ^ element.HasAttributes)
            {
                return false;
            }

            if (HasAttributes)
            {
                return Name == element.Name && AreAttributesEquals(element);
            }

            return Name == element.Name && Value == element.Value;
        }

        private bool AreChildrenEqual(XmlElement element)
        {
            if (Children.Count != element.Children.Count)
            {
                return false;
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (!element.children.Contains(children[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreAttributesEquals(XmlElement element) => attributes.Except(element.attributes).Count() == 0;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}", Name);
            ForEachAttribute((n, v) => sb.AppendFormat(" {0}='{1}'", n, v));
            sb.Append(">");
            if (Value != null) sb.Append(Value);
            return sb.ToString();
        }
    }
}