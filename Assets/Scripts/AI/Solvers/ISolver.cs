namespace Chrome
{
    public interface ISolver : IAssignable<IAgent>
    {
        void Build();

        void Bootup();
        void Evaluate();
        void Shutdown();
    }
}