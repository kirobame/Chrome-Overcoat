using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class FrenzyAbilityControl : InputControl, ILink<IIdentity>
    {
        #region IdentityLink
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        #endregion

        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree frenzyWeapon;
        [FoldoutGroup("Values"), SerializeField] private float heatCost;
        [FoldoutGroup("Values"), SerializeField] private bool lowHeatLock;

        private ComputeAimDirection aimCompute;

        void Awake()
        {

            frenzyWeapon.Bootup();

            
            var mask = LayerMask.GetMask("Environment", "Entity");
            var firAnchor = "view.fireAnchor".Reference<Transform>();
            var view = "view".Reference<Transform>();
            var collider = "self.collider".Reference<Collider>();
            aimCompute = new ComputeAimDirection("shootDir", mask, firAnchor, view, collider);

            var board = identity.Packet.Get<IBlackboard>();
            board.Set("frenzy.heatLock", lowHeatLock);
            board.Set("frenzy.heatCost", heatCost);

            frenzyWeapon.Bootup(identity.Packet);
        }

        public override void Bootup()
        {
            base.Bootup();
        }
        public override void Shutdown()
        {
            base.Shutdown();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                Activate();

            aimCompute.Update(identity.Packet);
            frenzyWeapon.Update(identity.Packet);
        }

        private void Activate()
        {
            identity.Packet.Set(true);
            var board = identity.Packet.Get<IBlackboard>();
            board.Get<BusyBool>("canSprint").business++;
        }
    }
}
