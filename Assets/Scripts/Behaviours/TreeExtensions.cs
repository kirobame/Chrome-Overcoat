namespace Chrome
{
    public static class TreeExtensions
    {
        public static bool IsChildOf(this Node node, Node supposedParent)
        {
            var parent = node.Parent;
            if (parent == supposedParent) return true;
            
            while (parent != null)
            {
                parent = parent.Parent;
                if (parent == supposedParent) return true;
            }

            return false;
        }

        public static TNode Append<TNode>(this TNode node, params Node[] nodes) where TNode : Node
        {
            node.Insert(nodes);
            return node;
        }
        public static TNode Mask<TNode>(this TNode node, int input) where TNode : Node
        {
            node.Input = input;
            return node;
        }
    }
}