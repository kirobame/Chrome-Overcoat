using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JeffInstaller : AgentInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private Weapon weapon;

        private Weapon runtimeWeapon;

        protected override void InstallDependenciesOn(Packet packet, IBlackboard board)
        {
            runtimeWeapon = Instantiate(weapon);
            runtimeWeapon.Build();

            board.Set(AgentRefs.WEAPON, runtimeWeapon);
            board.Set(WeaponRefs.BOARD, runtimeWeapon.Board);
        }
    }
}