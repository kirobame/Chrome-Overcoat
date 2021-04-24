using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public abstract class PhysicBody : MonoBehaviour, ILifebound
    {
        public event Action<CollisionHit<PhysicBody>> onCollision;
        
        public abstract Collider Collider { get; }

        public float Mass => mass;
        
        public Vector3 Velocity { get; private set; }
        public Vector3 Delta { get; private set; }

        [BoxGroup("Values"), SerializeField] private float mass;

        [HideInInspector] public Vector3 force;
        [HideInInspector] public Vector3 velocity;

        //--------------------------------------------------------------------------------------------------------------/

        public void Bootup()
        {
            Velocity = Vector3.zero;
            Delta = Vector3.zero;

            force = Vector3.zero;
            velocity = Vector3.zero;
        }
        public void Shutdown() { }
        
        void Update()
        {
            velocity += force * Time.deltaTime / mass;
            Delta = Move(velocity * Time.deltaTime);

            var hit = HandleCollisions();
            if (hit != null) onCollision?.Invoke(hit);

            Velocity = velocity;
            force = Vector3.zero;
        }

        protected abstract Vector3 Move(Vector3 delta);
        protected abstract CollisionHit<PhysicBody> HandleCollisions();
    }
}