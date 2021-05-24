using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public static class JeffManager
    {
        public static bool IsSynchAssaultReady()
        {
            if (Blackboard.Global.TryGet<Dictionary<Transform, bool>>("AI.Jeffs", out var jeffs))
            {
                foreach (var jeff in jeffs.Values)
                {
                    if (jeff == false)
                        return false;
                }
            }
            return true;
        }
        /*
        public static Vector3 GetNewPos(float degree, bool toRight, Packet packet)
        {
            var playerTr = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            var pivotRef = Refs.PIVOT.Reference<Transform>();

            if (!playerTr.IsValid(packet) || !pivotRef.IsValid(packet)) return Vector3.zero;

            Vector3 pos;
            if (toRight)
                pos = Quaternion.Euler(0, 30, 0) * pivotRef.Value.position;
            else
                pos = Quaternion.Euler(0, -30, 0) * pivotRef.Value.position;

            return pos;
        }*/
        public static Transform GetClosestJeff(Transform self)
        {
            Transform closestJeff = null;
            foreach (var jeff in Hive.Query<Agent>("JEFF"))
            {
                if (jeff.transform != self)
                {
                    if (closestJeff == null)
                        closestJeff = jeff.transform;
                    else
                        if (Vector3.Distance(jeff.transform.position, self.position) < Vector3.Distance(closestJeff.position, self.position))
                        closestJeff = jeff.transform;
                }
            }

            return closestJeff;
        }

        public static float GetSide(Transform self, Transform target, Transform aim)
        {
            var targetDir = (self.position - target.position).normalized;

            var dir = Vector3.Dot(aim.right, targetDir);


            //Debug.Log(self.gameObject.name + " " + target.gameObject.name + " -> " + dir);

            if (dir > 0.75f)
                return 1.0f;
            else if (dir < -0.75f)
                return -1.0f;
            else
                return 0.0f;
        }

    }
}