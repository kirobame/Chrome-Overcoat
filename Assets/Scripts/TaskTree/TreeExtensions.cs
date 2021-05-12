using System;

namespace Chrome
{
    public static class TT
    {
        public static INode IF_TRUE(INode node) => node.Mask(0b_0001);
        public static INode IF_FALSE(INode node) => node.Mask(0b_0010);

        public static INode WITH_PRIO(int value, INode node)
        {
            node.Priority = value;
            return node;
        }
        
        public static INode BIND_TO(int mask, INode node) => node.Mask(mask);
        public static INode BIND_TO<TEnum>(TEnum value, INode node) where TEnum : Enum => node.Mask(Convert.ToInt32(value));
        
        public static CompositeConditionalNode IF(ICondition condition) => new CompositeConditionalNode(condition);
        public static CompositeConditionalNode AND(this CompositeConditionalNode node, ICondition condition)
        {
            node.Add(ConditionalOperator.AND, condition);
            return node;
        } 
        public static CompositeConditionalNode OR(this CompositeConditionalNode node, ICondition condition)
        {
            node.Add(ConditionalOperator.OR, condition);
            return node;
        } 
    }
    
    public static class TreeExtensions
    {
        public static ICondition Inverse(this ICondition condition)
        {
            condition.Inverse = true;
            return condition;
        }
        public static ICondition Chain(this ICondition condition, ConditionalOperator op)
        {
            condition.Operator = op;
            return condition;
        }
        
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
    }
}