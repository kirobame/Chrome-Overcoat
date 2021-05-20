using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class KyleInstaller : AgentInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private Weapon weapon;

        private Weapon runtimeWeapon;

        protected override void InstallDependenciesOn(Packet packet, IBlackboard board)
        {
            runtimeWeapon = Instantiate(weapon);
            runtimeWeapon.Build();

            board.Set(AgentRefs.WEAPON, runtimeWeapon);
        }
    }
}