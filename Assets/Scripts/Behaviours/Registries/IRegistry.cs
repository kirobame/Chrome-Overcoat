namespace Chrome
{
    public interface IRegistry<out T> : IRegistry
    {
        T Value { get; }
    }

    public interface IRegistry
    {
        object RawValue { get; }
    }
}