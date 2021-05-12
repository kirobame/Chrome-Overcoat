namespace Chrome
{
    public interface IAssignable
    {
        object Value { get; }
    }
    public interface IAssignable<T> : IAssignable
    {
        T Owner { get; }
        
        void AssignTo(T owner);
    }
}