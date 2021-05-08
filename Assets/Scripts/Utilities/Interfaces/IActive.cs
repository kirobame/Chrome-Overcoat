using System;

namespace Chrome
{
    public interface IActive<out T>
    {
        event Action<T> onDestruction;
        
        bool IsActive { get; }
    }
}