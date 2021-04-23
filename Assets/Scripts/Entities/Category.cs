using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Category : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        [FoldoutGroup("Values"), SerializeField] private byte value;

        void Start() => identity.Packet.Set(value);
    }
}