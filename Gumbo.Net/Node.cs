using System.Collections.Immutable;

namespace Gumbo
{
    public abstract class Node
    {
        public GumboNodeType Type { get; }
        public GumboParseFlags ParseFlags { get; }
        public Node Parent { get; }
        public abstract ImmutableArray<Node> Children { get; }

        public Node(GumboNode node, Node parent)
        {
            Type = node.type;
            ParseFlags = node.parse_flags;
            Parent = parent;
        }
    }
}
