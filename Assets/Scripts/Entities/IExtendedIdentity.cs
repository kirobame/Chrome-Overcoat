namespace Chrome
{
    public interface IExtendedIdentity : IIdentity
    {
        IBlackboard Board { get; }
    }
}