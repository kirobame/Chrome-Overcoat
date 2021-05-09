namespace Chrome
{
    public struct EmptyValue<T> : IValue<T>
    {
        public object RawValue => null;
        public T Value
        {
            get => default;
            set { }
        }

        public void FillIn(Packet packet) { }
        public bool IsValid(Packet packet) => false;
    }
}