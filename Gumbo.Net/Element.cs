using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Gumbo
{
    [DebuggerDisplay("Type = {Type}, Tag = {Tag}")]
    public class Element : Node
    {
        public override ImmutableArray<Node> Children => _children.Value;
        public ImmutableArray<Attribute> Attributes => _attributes.Value;
        public string Value => _value.Value;
        public GumboTag Tag { get; }
        public GumboNamespaceEnum TagNamespace { get; }
        public GumboSourcePosition StartPosition { get; }
        public GumboSourcePosition EndPosition { get; }
        public string OriginalTag { get; }
        /// <summary>
        /// Returns lower-case name of every known HTML tag (according to the value of property <see cref="Tag"/>).
        /// For unknown tags returns empty string (where <see cref="Tag"/> is GUMBO_TAG_UNKNOWN).
        /// </summary>
        public string NormalizedTagName { get; }
        public string OriginalTagName { get; }
        public string OriginalEndTag { get; }
        readonly Lazy<ImmutableArray<Node>> _children;
        readonly Lazy<ImmutableArray<Attribute>> _attributes;
        readonly Lazy<string> _value;

        internal Element(GumboElementNode node, Node parent, GumboFactory factory)
            : base(node, parent)
        {
            _children = factory.CreateLazy(() => ImmutableArray.CreateRange(node.GetChildren().OrderBy(x => x.index_within_parent).Select(x => factory.CreateNode(x, this))));
            _attributes = factory.CreateLazy(() => ImmutableArray.CreateRange(node.GetAttributes().Select(x => factory.CreateAttribute(x, this))));
            _value = factory.CreateLazy(() => string.Concat(Children.Select(x => x is Element ? ((Element)x).Value : ((Text)x).Value)));
            StartPosition = node.element.start_pos;
            EndPosition = node.element.end_pos;
            Tag = node.element.tag;
            TagNamespace = node.element.tag_namespace;
            OriginalTag = NativeUtf8.StringFromNativeUtf8(node.element.original_tag.data, (int)node.element.original_tag.length);
            OriginalTagName = GetTagNameFromOriginalTag(node.element);
            OriginalEndTag = NativeUtf8.StringFromNativeUtf8(node.element.original_end_tag.data, (int)node.element.original_end_tag.length);
            NormalizedTagName = NativeUtf8.StringFromNativeUtf8(NativeMethods.gumbo_normalized_tagname(node.element.tag));
        }

        static string GetTagNameFromOriginalTag(GumboElement element)
        {
            var temp = element.original_tag;
            NativeMethods.gumbo_tag_from_original_text(ref temp);
            return temp.MarshalToString();
        }
    }
}
