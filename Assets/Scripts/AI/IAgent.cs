using System;
using System.Collections.Generic;

namespace Chrome
{
    public interface IAgent
    {  
        event Action<IAgent> onSpawn;
        event Action<IAgent> onDiscard;
        event Action<IAgent,string,string> onStreamChange;

        IIdentity Identity { get; }
        bool IsActive { get; }
        
        AgentDefinition Definition { get; }
        string Stream { get; }
        
        IGoal this[GoalDefinition definition] { get; }
        IEnumerable<IGoal> Goals { get; }
        
        void Interrupt(EventArgs args);
        
        void AddGoals(params IGoal[] goals);
        void RemoveGoals(GoalDefinition query);

        void Tag(string partialStream);
        void Erase(string partialStream);
    }
}