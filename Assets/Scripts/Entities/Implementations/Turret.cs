﻿using Flux;
using Flux.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class Turret : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        [SerializeField] private LineOfSight lineOfSight;
        
        [Space, SerializeField] private Transform aim;
        [SerializeField] private Transform fireAnchor;

        [Space, SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        
        private ITaskTree taskTree;

        void Start()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("aim", aim);
            board.Set("aim.fireAnchor", fireAnchor);
            
            identity.Packet.Set(lineOfSight);

            var playerColReference = "player.collider".Reference<Collider>(true);
            var fireAnchorReference = "aim.fireAnchor".Reference<Transform>();
            var aimReference = "aim".Reference<Transform>();
            
            taskTree = new RootNode();
            var conditionalNode = new CanSee(playerColReference, new PackettedValue<LineOfSight>());
            
            taskTree.Append(
                conditionalNode.Append(
                    new RootNode().Mask(0b_0001).Append(
                        new LookAt(playerColReference, aimReference)), 
                    new SimulatedClickInput(1.0f).Mask(0b_0001).Append(
                        new ComputeDirectionTo("shootDir", fireAnchorReference, playerColReference).Append(
                            new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, bulletPrefab, muzzleFlashPrefab).Append(
                                new Delay(0.33f))))),
                    new Delay(0.5f).Mask(0b_0010));
            
            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }

        void Update() => taskTree.Update(identity.Packet);
    }
}