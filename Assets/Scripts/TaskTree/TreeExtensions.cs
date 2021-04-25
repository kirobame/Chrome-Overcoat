namespace Chrome
{
    public static class TreeExtensions
    {
        public static bool HasChannel(this int mask, int channel) => (mask | channel) == mask;
        public static string ToBinary(this int value, int length = 4)
        {
            var buffer = new char[length];
 
            for (int i = length - 1; i >= 0 ; i--)
            {
                var mask = 1 << i;
                buffer[length - 1 - i] = (value & mask) != 0 ? '1' : '0';
            }
 
            return new string(buffer);
        }

        
        public static bool IsChildOf(this INode node, INode supposedParent)
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

        public static TNode Append<TNode>(this TNode node, params INode[] nodes) where TNode : INode
        {
            node.Insert(nodes);
            return node;
        }
        public static TNode Mask<TNode>(this TNode node, int input) where TNode : INode
        {
            node.Input = input;
            return node;
        }
        
        public static IValue<T> Cache<T>(this T value) => new CachedValue<T>(value);
        public static IValue<T> Reference<T>(this string path, bool globally = false)
        {
            if (globally) return new GloballyReferencedValue<T>(path);
            else return new LocallyReferencedValue<T>(path);
        }
    }
}