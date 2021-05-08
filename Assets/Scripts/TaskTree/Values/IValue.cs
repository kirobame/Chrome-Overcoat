namespace Chrome
{
    public interface IValue
    {
        object RawValue { get; }

        void FillIn(Packet packet);
        bool IsValid(Packet packet);
    }
    public interface IValue<out T> : IValue
    {
        T Value { get; }
    }
}