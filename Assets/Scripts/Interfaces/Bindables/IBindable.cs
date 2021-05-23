using System;

namespace Chrome
{
    public interface IBindable
    {
        HUDBinding Binding { get; }
    }
    public interface IBindable<T> : IBindable
    {
        event Action<T> onChange;
        
        T Value { get; set; }
    }
}