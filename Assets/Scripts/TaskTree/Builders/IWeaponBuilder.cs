namespace Chrome
{
    public interface IWeaponBuilder : ITreeBuilder
    {
        void InstallDependenciesOn(IBlackboard board);
        
        IBindable[] GetBindables();
    }
}