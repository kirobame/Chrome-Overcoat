using Flux.Audio;
using Flux.Data;
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
        [FoldoutGroup("Values"), SerializeField] private bool splatter;
        [FoldoutGroup("Values"), ShowIf("splatter"), SerializeField] private PoolableVfx bloodSplatterPrefab;

        public void Hit(IIdentity source, float amount, Packet packet)
        {
            sound.Play();
            
            if (splatter && packet.TryGet<RaycastHit>(out var hit))
            {
                var impactPool = Repository.Get<VfxPool>(Pool.Impact);
                var vfxInstance = impactPool.RequestSingle(bloodSplatterPrefab);

                vfxInstance.transform.position = hit.point;
                vfxInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
                vfxInstance.Play();
            }
        }
    }
}