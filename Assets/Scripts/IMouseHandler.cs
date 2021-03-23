using System.Collections;
using System.Collections.Generic;

namespace Chrome
{
    public interface IMouseHandler
    {
        void Lock();
        void Unlock();

        void Bootup(float rotation);
        float Process(float delta);
    }
}