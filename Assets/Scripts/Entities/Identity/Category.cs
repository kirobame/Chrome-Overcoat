using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Category : MonoBehaviour, IInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private byte value;
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(value);
    }
}