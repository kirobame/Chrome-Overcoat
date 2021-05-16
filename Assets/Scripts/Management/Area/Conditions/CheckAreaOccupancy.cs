using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CheckAreaOccupancy : Condition
    {
        [SerializeField] private Comparison comparison;
        [SerializeField] private int value;

        public override bool Check(Packet packet)
        {
            if (packet.TryGet<Area>(out var area))
            {
                switch (comparison)
                {
                    case Comparison.EQUAL:
                        return area.Occupancy == value;

                    case Comparison.GREATER:
                        return area.Occupancy > value;

                    case Comparison.GREATER_OR_EQUAL:
                        return area.Occupancy >= value;

                    case Comparison.LESSER:
                        return area.Occupancy < value;

                    case Comparison.LESSER_OR_EQUAL:
                        return area.Occupancy <= value;
                }

                return false;
            }
            else return false;
        }
    }
}