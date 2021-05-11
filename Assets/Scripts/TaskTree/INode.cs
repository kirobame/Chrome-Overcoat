using System;
using System.Collections.Generic;

namespace Chrome
{
    public interface INode : IComparable<INode>
    {
        bool IsDone { get; }
        bool IsLocked { get; }
        NodeState State { get; }
        
        int Priority { get; }
        int Input { get; set; }
        int Output { get; }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        INode Parent { get; set; }
        IReadOnlyList<INode> Children { get; }

        void Bootup(Packet packet);

        void Prepare(Packet packet);
        IEnumerable<INode> Use(Packet packet);

        void Close(Packet packet);
        void Shutdown(Packet packet);
        
        //--------------------------------------------------------------------------------------------------------------/

        void Insert(IEnumerable<INode> nodes);
        
        void Cut(IEnumerable<INode> nodes);
        void Cut(Predicate<INode> predicate);
    }
}