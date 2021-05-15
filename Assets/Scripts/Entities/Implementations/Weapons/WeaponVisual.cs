using UnityEngine;

namespace Chrome
{
    public class WeaponVisual : MonoBehaviour, IInstaller
    {
        public SkinnedMeshRenderer Renderer => renderer;
        
        [SerializeField] private new SkinnedMeshRenderer renderer;

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}