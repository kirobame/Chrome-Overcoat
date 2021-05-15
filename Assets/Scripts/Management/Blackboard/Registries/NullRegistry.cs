namespace Chrome
{
    public class NullRegistry : IRegistry
    {
        public object RawValue => null;
        
        public void Set(object rawValue) { }
        public IRegistry Copy() => new NullRegistry();
    }
}