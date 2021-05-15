namespace Chrome
{
    public class WrapperRegistry<T> : IRegistry<T>
    {
        public WrapperRegistry(T value) => this.value = value;
        
        public object RawValue => value;
        
        public T Value => value;
        private T value;

        public void Set(object rawValue) => value = (T)rawValue;
        public IRegistry Copy() => new WrapperRegistry<T>(value);
    }
}