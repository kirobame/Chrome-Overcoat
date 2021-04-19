using Flux;
using Flux.Event;

namespace Chrome
{
    public class WrapperSendbackArgs<T> : SendbackArgs, IWrapper<T>
    {
        public WrapperSendbackArgs(T value) => Value = value;
        
        public T Value { get; protected set; }
    }
}