namespace Chrome
{
    public struct EmptyValue<T> : IValue<T>
    {
        public object RawValue
        {
            get => null;
            set { }
        }
        public T Value
        {
            get => default;
            set { }
        }

        public bool IsValid(Packet packet) => false;
    }
}