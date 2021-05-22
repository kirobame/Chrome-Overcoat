namespace Chrome
{
    public abstract class Condition : ICondition
    {
        public virtual void Bootup(Packet packet) { }
        public virtual void Open(Packet packet) { }
        public virtual void Prepare(Packet packet) { }

        public abstract bool Check(Packet packet);

        public virtual void Close(Packet packet) { }
        public virtual void Shutdown(Packet packet) { }
    }
}