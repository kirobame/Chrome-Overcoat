namespace Chrome
{
    public class BindableState<T> : Bindable<T>
    {
        public BindableState(HUDBinding binding) : base(binding) { }
        public BindableState(HUDBinding binding, T initialValue) : base(binding, initialValue) { }

        public bool HasValue { get; private set; }

        public void Set(T value)
        {
            HasValue = true;
            Value = value;
        }
        public void Discard()
        {
            HasValue = false;
            Value = default;
        }
    }
}