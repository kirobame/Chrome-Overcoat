using System;

namespace Chrome
{
    public interface IListener<T> : IActive<IListener<T>>
    {
        bool IsListeningTo(EventArgs args);
        void Execute(Token token);
    }
}