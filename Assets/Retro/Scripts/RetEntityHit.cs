using Flux.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetEntityHit : MonoBehaviour, IDamageable, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        protected IIdentity identity;
        
        [FoldoutGroup("Values"), SerializeField] private AudioPackage sound;

        public void Hit(IIdentity source, float amount, Packet packet) => sound.Play();
    }
}