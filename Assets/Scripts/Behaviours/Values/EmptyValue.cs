namespace Chrome
{
    public struct EmptyValue<T> : IValue<T>
    {
        public object RawValue => null;
        public T Value => default;

        public bool IsValid(Packet packet) => false;
    }
}