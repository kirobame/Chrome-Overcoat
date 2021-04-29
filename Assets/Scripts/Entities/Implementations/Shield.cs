using System;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Shield : MonoBehaviour, IDamageable, ILifebound, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        [BoxGroup("Dependencies"), SerializeField] private Lifetime link;
        [FoldoutGroup("References"), SerializeField] private MeshRenderer mesh;
        [FoldoutGroup("References"), SerializeField] private MeshCollider col;

        [FoldoutGroup("Values"), SerializeField] private float maxHealth;
        private float health;

        private bool isBroken = false;
        public bool IsBroken() { return isBroken; }

        void Awake() => Bootup();
        void Start() { }

        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var difference = health - amount;
            var damage = difference < 0 ? amount + difference : amount;
            health -= damage;

            var type = identity.Packet.Get<byte>();

            Events.ZipCall<byte, float>(GaugeEvent.OnDamageInflicted, type, damage);

            if (health <= 0) link.End();
            //if (health <= 0) Shutdown();
        }

        public void Up()
        {
            if (!mesh.enabled)
                mesh.enabled = true;
            if (!col.enabled)
                col.enabled = true;
        }
        public void Down()
        {
            if (mesh.enabled)
                mesh.enabled = false;
            if (col.enabled)
                col.enabled = false;
        }

        public void Bootup()
        {
            //Debug.Log("Shield bootup");
            health = maxHealth;
            isBroken = false;
        }

        public void Shutdown()
        {
            isBroken = true;
        }
    }
}
