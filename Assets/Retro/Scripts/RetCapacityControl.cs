using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetCapacityControl : MonoBehaviour, ILink<IIdentity>
    { 
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        [FoldoutGroup("Dependencies"), SerializeField] private Transform fireAnchor;
        
        [FoldoutGroup("Values"), SerializeField] private GenericPoolable projectilePrefab;
        [FoldoutGroup("Values"), SerializeField] private float cost;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx muzzleFlashPrefab;

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.R)) return;

            var gauge = Repository.Get<RetGauge>(RetReference.Gauge);
            gauge.Modify(-cost);
            
            var direction = fireAnchor.forward;
            identity.Packet.Set(direction);
            
            var muzzleFlashPool = Repository.Get<VfxPool>(Pool.MuzzleFlash);
            var muzzleFlashInstance = muzzleFlashPool.RequestSinglePoolable(muzzleFlashPrefab);

            muzzleFlashInstance.transform.localScale = Vector3.one;
            muzzleFlashInstance.transform.parent = fireAnchor;
            muzzleFlashInstance.transform.position = fireAnchor.position;
            muzzleFlashInstance.transform.rotation = Quaternion.LookRotation(fireAnchor.forward);
            muzzleFlashInstance.Value.Play();
            
            var projectilePool = Repository.Get<GenericPool>(Pool.Projectile);
            var projectileInstance = projectilePool.CastSingle<Projectile>(projectilePrefab);
            
            projectileInstance.Shoot(identity, fireAnchor.position, direction, identity.Packet);
        }
    }
}