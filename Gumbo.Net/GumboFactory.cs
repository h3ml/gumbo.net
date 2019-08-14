using System;
using System.Collections.Generic;
using System.Linq;

namespace Gumbo
{
    internal class GumboFactory
    {
        readonly LazyFactory _lazyFactory;
        readonly Dictionary<string, List<Element>> _marshalledElementsByIds = new Dictionary<string, List<Element>>(StringComparer.OrdinalIgnoreCase);

        public GumboFactory(LazyFactory lazyFactory) => _lazyFactory = lazyFactory;

        public Node CreateNode(GumboNode node, Node parent = null)
        {
            switch (node.type)
            {
                case GumboNodeType.GUMBO_NODE_DOCUMENT: return new Document((GumboDocumentNode)node, this);
                case GumboNodeType.GUMBO_NODE_ELEMENT:
                case GumboNodeType.GUMBO_NODE_TEMPLATE: return new Element((GumboElementNode)node, parent, this);
                case GumboNodeType.GUMBO_NODE_TEXT:
                case GumboNodeType.GUMBO_NODE_CDATA:
                case GumboNodeType.GUMBO_NODE_COMMENT:
                case GumboNodeType.GUMBO_NODE_WHITESPACE: return new Text((GumboTextNode)node, parent);
                default: throw new NotImplementedException($"Node type '{node.type}' is not implemented");
            }
        }

        public Attribute CreateAttribute(GumboAttribute attribute, Element parent)
        {
            var r = new Attribute(attribute, parent);
            if (string.Equals(r.Name, "id", StringComparison.OrdinalIgnoreCase))
                AddElementById(r.Value, parent);
            return r;
        }

        public Lazy<T> CreateLazy<T>(Func<T> factoryMethod) => _lazyFactory.Create(factoryMethod);

        public Element GetElementById(string id) => _marshalledElementsByIds.TryGetValue(id, out var elements) ? elements.FirstOrDefault() : null;

        void AddElementById(string id, Element element)
        {
            if (!_marshalledElementsByIds.TryGetValue(id, out var elements))
                _marshalledElementsByIds.Add(id, elements = new List<Element>());
            elements.Add(element);
        }
    }
}
