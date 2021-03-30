using System;
using Flux;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class MuzzleFlash : GunPart
    {
        [SerializeField] private PoolableVfx muzzleFlash;

        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            var muzzleFlashPool = Repository.Get<VfxPool>(Pool.MuzzleFlash);
            var muzzleFlashInstance = muzzleFlashPool.RequestSinglePoolable(muzzleFlash);

            if (args is IWrapper<float> sizeWrapper) muzzleFlashInstance.transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 1.0f, sizeWrapper.Value);
            else muzzleFlashInstance.transform.localScale = Vector3.one;

            muzzleFlashInstance.transform.parent = Control.Firepoint;
            muzzleFlashInstance.transform.position = aim.firepoint;
            muzzleFlashInstance.transform.rotation = Quaternion.LookRotation(aim.firingDirection);
            muzzleFlashInstance.Value.Play();
            
            return args;
        }
    }
}