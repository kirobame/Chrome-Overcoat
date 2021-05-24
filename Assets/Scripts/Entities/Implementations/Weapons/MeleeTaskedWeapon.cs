using UnityEngine;

namespace Chrome
{
    [CreateAssetMenu(fileName = "NewMeleeTaskedWeapon", menuName = "Chrome Overcoat/Weapons/Melee Tasked")]
    public class MeleeTaskedWeapon : TaskedWeapon
    {
        public override bool IsMelee => true;
    }
}