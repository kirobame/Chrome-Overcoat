using UnityEngine;

namespace Chrome
{
    public class SubArea : MonoBehaviour, IInstaller
    {
        [SerializeField] private new BoxCollider collider;

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.AddElement(collider);
    }
}