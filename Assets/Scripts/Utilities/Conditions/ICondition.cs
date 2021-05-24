namespace Chrome
{
    public interface ICondition
    {
        void Bootup(Packet packet);
        void Open(Packet packet);

        void Prepare(Packet packet);
        bool Check(Packet packet);

        void Close(Packet packet);
        void Shutdown(Packet packet);
    }
}