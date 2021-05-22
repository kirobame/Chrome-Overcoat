using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public static class CoverSystem
    {
        private static Dictionary<Area, List<CoverSpot>> spots;
        private static HashSet<CoverSpot> occupiedSpots;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootup()
        {
            spots = new Dictionary<Area, List<CoverSpot>>();
            occupiedSpots = new HashSet<CoverSpot>();
        }

        public static void Register(CoverSpot spot)
        {
            if (spots.TryGetValue(spot.Area, out var list)) list.Add(spot);
            else spots.Add(spot.Area, new List<CoverSpot>() { spot });
        }

        public static bool Request(Area area, Vector3 current, Vector3 view, float range, float acceptance, Collider target, out CoverSpot spot)
        {
            spot = null;
            if (!spots.TryGetValue(area, out var list)) return false;

            var mask = LayerMask.GetMask("Environment");
            var shortestDistance = range;

            var index = 0;
            foreach (var candidate in list)
            {
                index++;
                
                var position = target.transform.position;
                position.y = candidate.Position.y;

                var direction = Vector3.Normalize(candidate.Position - position);
                if (Vector3.Dot(candidate.Orientation, direction) > acceptance) continue;
                
                if (Vector3.Dot(Vector3.Normalize(position - current), direction) > 0) continue;

                var distance = Vector3.Distance(candidate.Position, current);
                if (distance >= shortestDistance) continue;

                var point = candidate.Position + view;
                if (!point.CanSee(target, mask)) continue;

                shortestDistance = distance;
                spot = candidate;
            }

            if (spot == null) return false;

            list.Remove(spot);
            occupiedSpots.Add(spot);
            
            return true;
        }
        public static void Discard(CoverSpot spot)
        {
            if (!occupiedSpots.Remove(spot)) return;
            spots[spot.Area].Add(spot);
        }
    }
}