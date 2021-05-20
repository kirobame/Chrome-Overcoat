using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public static class CoverSystem
    {
        private static Dictionary<Area, CoverSpot> spots;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootup() => spots = new Dictionary<Area, CoverSpot>();
        
        public static void Register(CoverSpot spot)
        {
            
        }

        public static CoverSpot Request(Area area, Vector3 current, Vector3 target)
        {
            return null;
        }
        public static void Discard(CoverSpot spot)
        {
            
        }
    }
}