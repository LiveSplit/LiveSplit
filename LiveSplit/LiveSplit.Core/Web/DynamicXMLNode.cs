using System.Dynamic;
using System.Xml;

namespace LiveSplit.Web
{
    public static class XMLExtensions
    {
        public static dynamic ToDynamic(this XmlElement element)
        {
            return new DynamicXMLElement(element);
        }
    }

    internal class DynamicXMLAttributesCollection : DynamicObject
    {
        private XmlAttributeCollection Attributes { get; set; }

        public DynamicXMLAttributesCollection(XmlAttributeCollection attributes)
        {
            Attributes = attributes;
        }

        public override string ToString()
        {
            return Attributes.ToString();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = Attributes[binder.Name];
                return true;
            }
            catch { }

            result = null;
            return false;
        }
    }

    internal class DynamicXMLElement : DynamicObject
    {
        private XmlElement Element { get; set; }

        public DynamicXMLElement(XmlElement element)
        {
            Element = element;
        }

        public override string ToString()
        {
            return Element.ToString();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                if (binder.Name == "Attributes")
                {
                    result = new DynamicXMLAttributesCollection(Element.Attributes);
                    return true;
                }

                var memberElement = Element[binder.Name];
                if (memberElement.HasChildNodes)
                {
                    result = new DynamicXMLElement(memberElement);
                    return true;
                }
                else
                {
                    result = memberElement.InnerText;
                    return true;
                }
            }
            catch { }

            result = null;
            return false;
        }
    }
}
