using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetLoot : MonoBehaviour, ILifebound
    {
        [FoldoutGroup("Dependencies"), SerializeField] private Transform source;
        
        [FoldoutGroup("Values"), SerializeField] private GenericPoolable pickupPrefab;
        [FoldoutGroup("Values"), SerializeField] private Vector2 angle;
        [FoldoutGroup("Values"), SerializeField] private Vector2 ejection;
        [FoldoutGroup("Values"), SerializeField] private Vector2 torque;

        public void Bootup() { }
        public void Shutdown()
        {
            var pickupPool = Repository.Get<GenericPool>(RetReference.PickupPool);
            var pickupInstance = pickupPool.CastSingle<RetGunPickup>(pickupPrefab);
            pickupInstance.Bootup();

            var angle = Random.Range(this.angle.x, this.angle.y) * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
            
            var y = Random.Range(0.0f, 360.0f);
            direction = Vector3.Normalize(Quaternion.AngleAxis(y, Vector3.up) * direction);
            
            pickupInstance.transform.position = source.position;
            pickupInstance.Rigidbody.velocity = Vector3.zero;
            pickupInstance.Rigidbody.AddForce(direction * Random.Range(ejection.x, ejection.y), ForceMode.Impulse);
            pickupInstance.Rigidbody.AddTorque(Random.onUnitSphere * Random.Range(torque.x, torque.y), ForceMode.Impulse);
        }
    }
}