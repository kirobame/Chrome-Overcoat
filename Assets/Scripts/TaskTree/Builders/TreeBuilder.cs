using System;

namespace Chrome
{
    public abstract class TreeBuilder : ITreeBuilder
    {
        public abstract ITaskTree Build();
        
        public virtual void Bootup(Packet packet) { }
        public virtual void Shutdown(Packet packet) { }
    }
}