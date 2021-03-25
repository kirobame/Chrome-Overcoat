using System.Collections;
using System.Collections.Generic;

namespace Chrome
{
    public interface IViewAxisHandler
    {
        void Lock();
        void Unlock();

        void Set(float rotation);
        float Process(float delta);
    }
}