using System;
using System.Linq;
using System.Xml.XPath;

namespace Gumbo.Xml
{
    internal class GumboNavigator : XPathNavigator
    {
        class NavigatorState : IEquatable<NavigatorState>
        {
            public Node Node { get; private set; }
            public Attribute Attribute { get; private set; }
            public NavigatorState(Attribute attribute) => Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            public NavigatorState(Node node) => Node = node ?? throw new ArgumentNullException(nameof(node));

            public void SetCurrent(Attribute attribute)
            {
                Node = null;
                Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            }

            public void SetCurrent(Node node)
            {
                Node = node ?? throw new ArgumentNullException(nameof(node));
                Attribute = null;
            }

            public bool Equals(NavigatorState other) => Node == other.Node && Attribute == other.Attribute;
        }

        readonly NavigatorState _state;
        readonly Gumbo _gumbo;

        public override string BaseURI => throw new NotImplementedException();

        public GumboNavigator(Gumbo gumbo, Node node)
        {
            _gumbo = gumbo;
            _state = new NavigatorState(node);
        }
        public GumboNavigator(Gumbo gumbo, Attribute attribute)
        {
            _gumbo = gumbo;
            _state = new NavigatorState(attribute);
        }

        public override bool IsEmptyElement => _state.Node != null
            && _state.Node.Type == GumboNodeType.GUMBO_NODE_ELEMENT
            && !_state.Node.Children.Any();

        public override bool IsSamePosition(XPathNavigator other) => !(other is GumboNavigator otherGumboNav) ? false : _state.Equals(otherGumboNav._state);

        public override string LocalName => _state.Attribute != null
            ? _state.Attribute.Name
            : _state.Node is Element element
            ? !string.IsNullOrEmpty(element.OriginalTagName) ? element.OriginalTagName.Split(':').Last() : element.NormalizedTagName
            : string.Empty;

        public override bool MoveTo(XPathNavigator other) => !(other is GumboNavigator otherGumboNav) ? false : _state == otherGumboNav._state;

        public override bool MoveToFirstAttribute()
        {
            if (!(_state.Node is Element element))
                return false;
            var firstAttr = element.Attributes.FirstOrDefault();
            if (firstAttr == null)
                return false;
            _state.SetCurrent(firstAttr);
            return true;
        }

        public override bool MoveToFirstChild()
        {
            if (_state.Node == null)
                return false;
            var child = _state.Node.Children.FirstOrDefault();
            if (child == null)
                return false;
            _state.SetCurrent(child);
            return true;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) => throw new NotImplementedException();

        public override bool MoveToId(string id)
        {
            _gumbo.MarshalAll();
            var element = _gumbo.GetElementById(id);
            if (element == null)
                return false;
            _state.SetCurrent(element);
            return true;
        }

        public override bool MoveToNext()
        {
            if (_state.Node == null || _state.Node.Parent == null)
                return false;
            var parent = _state.Node.Parent;
            var nextIndex = parent.Children.IndexOf(_state.Node) + 1;
            if (nextIndex >= parent.Children.Length)
                return false;
            _state.SetCurrent(parent.Children[nextIndex]);
            return true;
        }

        public override bool MoveToNextAttribute()
        {
            if (_state.Attribute == null)
                return false;
            var parent = _state.Attribute.Parent;
            var nextIndex = parent.Attributes.IndexOf(_state.Attribute) + 1;
            if (nextIndex >= parent.Attributes.Length)
                return false;
            _state.SetCurrent(parent.Attributes[nextIndex]);
            return true;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) => throw new NotImplementedException();

        public override bool MoveToParent()
        {
            if (_state.Node == null || _state.Node.Parent == null)
                return false;
            _state.SetCurrent(_state.Node.Parent);
            return true;
        }

        public override bool MoveToPrevious()
        {
            if (_state.Node == null || _state.Node.Parent == null)
                return false;
            var parent = _state.Node.Parent;
            var nextIndex = parent.Children.IndexOf(_state.Node) - 1;
            if (nextIndex < 0)
                return false;
            _state.SetCurrent(parent.Children[nextIndex]);
            return true;
        }

        public override string Name => _state.Attribute != null
            ? _state.Attribute.OriginalName : _state.Node is Element element
            ? element.OriginalTagName : string.Empty;

        public override System.Xml.XmlNameTable NameTable => throw new NotImplementedException();

        public override string NamespaceURI => string.Empty;

        public override XPathNodeType NodeType
        {
            get
            {
                if (_state.Attribute != null)
                    return XPathNodeType.Attribute;
                System.Diagnostics.Debug.Assert(_state.Node != null);
                switch (_state.Node.Type)
                {
                    case GumboNodeType.GUMBO_NODE_DOCUMENT: return XPathNodeType.Root;
                    case GumboNodeType.GUMBO_NODE_ELEMENT:
                    case GumboNodeType.GUMBO_NODE_TEMPLATE: return XPathNodeType.Element;
                    case GumboNodeType.GUMBO_NODE_TEXT:
                    case GumboNodeType.GUMBO_NODE_CDATA: return XPathNodeType.Text;
                    case GumboNodeType.GUMBO_NODE_COMMENT: return XPathNodeType.Comment;
                    case GumboNodeType.GUMBO_NODE_WHITESPACE: return XPathNodeType.Whitespace;
                    default: throw new NotImplementedException();
                }
            }
        }

        public override string Prefix
        {
            get
            {
                if (_state.Attribute != null)
                    switch (_state.Attribute.Namespace)
                    {
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_NONE: return string.Empty;
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_XLINK: return "xlink";
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_XML: return "xml";
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_XMLNS: return "xmlns";
                        default: throw new NotSupportedException($"Namespace '{_state.Attribute.Namespace}' is not supported");
                    }
                return string.Empty; // namespaces are implicit in html/svg/mathml
            }
        }

        public override string Value => _state.Attribute != null
            ? _state.Attribute.Value : _state.Node is Element element
            ? element.Value : _state.Node is Text text
            ? text.Value : string.Empty;

        public override XPathNavigator Clone() => _state.Node != null
            ? new GumboNavigator(_gumbo, _state.Node) : _state.Attribute != null
            ? new GumboNavigator(_gumbo, _state.Attribute) : throw new InvalidOperationException();
    }
}
