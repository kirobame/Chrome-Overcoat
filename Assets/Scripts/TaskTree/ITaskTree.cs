using System.Collections.Generic;

namespace Chrome
{
    public interface ITaskTree : INode
    {
        IEnumerable<INode> UpdatedNodes { get; }
        IEnumerable<Branch> Branches { get; }

        bool IsUpdating(INode node);
        
        void Command(Packet packet, ICommand command);
        void ActualizeCommands(Packet packet);
        
        void AddOutputChannel(Packet packet, int value);
        void RemoveOutputChannel(Packet packet, int value);
        void ChangeOutputMask(Packet packet, int value);
    }
}