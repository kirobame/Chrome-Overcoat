namespace Chrome
{
    public interface ILifebound : IActive<ILifebound>
    {
        void Bootup();
        void Shutdown();
    }
}