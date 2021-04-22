namespace Chrome
{
    public interface IValue
    {
        object RawValue { get; }

        bool IsValid(Packet packet);
    }
    public interface IValue<out T> : IValue
    {
        T Value { get; }
    }
}