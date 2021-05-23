namespace Chrome
{
    public interface IWeaponBuilder : ITreeBuilder
    {
        IBindable[] GetBindables();
    }
}