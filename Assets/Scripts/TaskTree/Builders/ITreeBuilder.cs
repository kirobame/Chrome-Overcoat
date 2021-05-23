using System;

namespace Chrome
{
    public interface ITreeBuilder
    {
        ITaskTree Build();

        void Bootup(Packet packet);
        void Shutdown(Packet packet);
    }
}