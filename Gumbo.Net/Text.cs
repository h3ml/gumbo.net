using System.Collections.Immutable;
using System.Diagnostics;

namespace Gumbo
{
    [DebuggerDisplay("Type = {Type}, Value = {Value}")]
    public class Text : Node
    {
        public string Value { get; }
        public GumboSourcePosition StartPosition { get; }
        public override ImmutableArray<Node> Children => ImmutableArray.Create<Node>();

        internal Text(GumboTextNode node, Node parent) : base(node, parent)
        {
            Value = NativeUtf8.StringFromNativeUtf8(node.text.text);
            StartPosition = node.text.start_pos;
        }
    }
}
