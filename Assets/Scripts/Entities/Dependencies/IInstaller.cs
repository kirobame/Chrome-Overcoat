namespace Chrome
{
    public interface IInstaller
    {
        int Priority { get; }
        
        void InstallDependenciesOn(Packet packet);
    }
}