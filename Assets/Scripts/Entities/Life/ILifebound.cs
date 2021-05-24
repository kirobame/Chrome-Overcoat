namespace Chrome
{
    public interface ILifebound : IActive<ILifebound>
    {
        void Bootup(byte code);
        void Shutdown(byte code);
    }
}