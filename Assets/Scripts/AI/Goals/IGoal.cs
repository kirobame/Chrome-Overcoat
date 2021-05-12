namespace Chrome
{
    public interface IGoal : IAssignable<IAgent>
    {
        GoalDefinition Definition { get; }
        
        bool IsActive { get; set; }
        bool IsDirty { get; set; }
        bool IsAccomplished { get; set; }
        
        void Reset();
        void Evaluate();
    }
}