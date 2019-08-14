using System;
using System.Linq;
using System.Xml.Linq;

namespace Gumbo
{
    public static class GumboExtensions
    {
        public static XDocument ToXDocument(this GumboDocumentNode docNode) => (XDocument)CreateXNode(docNode);

        static XNode CreateXNode(GumboNode node)
        {
            switch (node.type)
            {
                case GumboNodeType.GUMBO_NODE_DOCUMENT: return new XDocument(((GumboDocumentNode)node).GetChildren().Select(CreateXNode));
                case GumboNodeType.GUMBO_NODE_ELEMENT:
                case GumboNodeType.GUMBO_NODE_TEMPLATE:
                    var elementNode = (GumboElementNode)node;
                    var elementName = GetName(elementNode.element.tag);
                    var attributes = elementNode.GetAttributes().Select(x => new XAttribute(NativeUtf8.StringFromNativeUtf8(x.name), NativeUtf8.StringFromNativeUtf8(x.value)));
                    return new XElement(elementName, attributes, elementNode.GetChildren().Select(CreateXNode));
                case GumboNodeType.GUMBO_NODE_TEXT: return new XText(NativeUtf8.StringFromNativeUtf8(((GumboTextNode)node).text.text));
                case GumboNodeType.GUMBO_NODE_CDATA: return new XCData(NativeUtf8.StringFromNativeUtf8(((GumboTextNode)node).text.text));
                case GumboNodeType.GUMBO_NODE_COMMENT: return new XComment(NativeUtf8.StringFromNativeUtf8(((GumboTextNode)node).text.text));
                case GumboNodeType.GUMBO_NODE_WHITESPACE: return new XText(NativeUtf8.StringFromNativeUtf8(((GumboTextNode)node).text.text));
                default: throw new NotImplementedException($"Node type '{node.type}' is not implemented");
            }
        }

        static string GetName(GumboTag tag) => tag.ToString().Substring("GUMBO_TAG_".Length).ToLower().Replace('_', '-');
    }
}
