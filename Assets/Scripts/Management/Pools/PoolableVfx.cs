using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class PoolableVfx : Poolable<ParticleSystem>
    {
        void Update()
        {
            if (!Value.isPlaying) gameObject.SetActive(false);
        }
    }
}