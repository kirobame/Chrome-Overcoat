using System;
using System.Collections;
using System.Collections.Generic;
using Flux;
using Flux.Data;
using Flux.Feedbacks;
using UnityEngine;

namespace Chrome
{
    [Serializable, Path("Visual Effects")]
    public class PlayVfx : Effect
    {
        [SerializeField] private PoolableVfx prefab;
        [SerializeField] private Pool source;
        
        protected override void OnUpdate(EventArgs args)
        {
            if (!(args is IWrapper<Vector3> wrapper))
            {
                IsDone = true;
                return;
            }
            
            var pool = Repository.Get<VfxPool>(source);
            var instance = pool.RequestSingle(prefab);

            instance.transform.position = wrapper.Value;
            instance.Play();

            IsDone = true;
        }
    }
}