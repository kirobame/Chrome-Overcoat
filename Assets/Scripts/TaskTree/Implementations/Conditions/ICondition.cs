namespace Chrome
{
    public interface ICondition
    {
        bool Inverse { get; set; }
        ConditionalOperator Operator { get; set; }
        
        void Bootup(Packet packet);
        void Open(Packet packet);

        void Start(Packet packet);
        bool Check(Packet packet);

        void Close(Packet packet);
        void Shutdown(Packet packet);
    }
}