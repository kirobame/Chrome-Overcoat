using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Group("LateUpdate", "Any/Any"), Order("LateUpdate/DataSync", "Any/Any")]
    public class DataSync : Flux.EDS.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, Move move) => Entities.MarkDirty<CharacterController>(entity, move));
        }
    }
}