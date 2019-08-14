using System;
using System.Collections.Immutable;
using System.Linq;

namespace Gumbo
{
    public class Document : Node
    {
        public Element Root => (Element)Children.FirstOrDefault();
        public bool HasDocType { get; }
        public string Name { get; }
        public string PublicIdentifier { get; }
        public string SystemIdentifier { get; }
        public GumboQuirksModeEnum DocTypeQuirksMode { get; }
        public override ImmutableArray<Node> Children => _children.Value;
        readonly Lazy<ImmutableArray<Node>> _children;

        internal Document(GumboDocumentNode node, GumboFactory factory)
            : base(node, null)
        {
            _children = factory.CreateLazy(() => ImmutableArray.CreateRange(node.GetChildren().OrderBy(x => x.index_within_parent).Select(x => factory.CreateNode(x, this))));
            HasDocType = node.document.has_doctype;
            Name = NativeUtf8.StringFromNativeUtf8(node.document.name);
            PublicIdentifier = NativeUtf8.StringFromNativeUtf8(node.document.public_identifier);
            SystemIdentifier = NativeUtf8.StringFromNativeUtf8(node.document.system_identifier);
            DocTypeQuirksMode = node.document.doc_type_quirks_mode;
        }
    }
}
