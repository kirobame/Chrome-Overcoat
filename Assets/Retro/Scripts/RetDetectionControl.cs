using System;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetDetectionControl : MonoBehaviour
    {
        public event Action<Collider> onTargetEntry;
        public event Action<Collider> onTargetLeft;

        public IEnumerable<Collider> Targets => inRange;

        [FoldoutGroup("Dependencies"), SerializeField] private RetGunControl gun;
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterBody body;
        [FoldoutGroup("Dependencies"), SerializeField] private MeshFilter filter;
        [FoldoutGroup("Dependencies"), SerializeField] private new MeshRenderer renderer;

        [FoldoutGroup("Values"), SerializeField] private int quality;

        private bool hasBeenBootedUp;
        
        private HashSet<Collider> inRange;
        private HashSet<Collider> cache;
        
        private Vector3[] vertices;
        private Vector2[] UVs;
        private Vector3[] normals;
        private int[] triangles;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            inRange = new HashSet<Collider>();
            cache = new HashSet<Collider>();
            
            gun.onGunSwitch += Bootup;
        }
        void OnDestroy() => gun.onGunSwitch -= Bootup;

        void Bootup(RetGun gun)
        {
            var definition = ComputeDefinition();
            var length = definition + 1;
            
            vertices = new Vector3[length];
            vertices[0] = Vector3.zero;
            
            UVs = new Vector2[length];
            UVs[0] = Vector2.zero;
            
            normals = new Vector3[length];
            normals[0] = Vector3.up;
            
            triangles = new int[(definition - 1) * 3];
            
            var mesh = new Mesh();
            filter.mesh = mesh;

            hasBeenBootedUp = true;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (!hasBeenBootedUp) return;
            
            if (!body.IsGrounded)
            {
                if (inRange.Any())
                {
                    foreach (var target in inRange) onTargetLeft?.Invoke(target);
                    inRange.Clear();
                }
                
                renderer.enabled = false;
                return;
            }
            else renderer.enabled = true;
            
            cache.Clear();
            var definition = ComputeDefinition();

            var baseAngle = -gun.Current.Spread * 0.5f + 90.0f;
            var step = gun.Current.Spread / definition;
            for (var i = 0; i < definition; i++)
            {
                if (i < definition - 1)
                {
                    var baseIndex = i * 3;
                    triangles[baseIndex] = 0;
                    triangles[baseIndex + 1] = i + 1;
                    triangles[baseIndex + 2] = i + 2;
                }

                var angle = (baseAngle + step * i) * Mathf.Deg2Rad;
                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);

                Vector3 point;
                var direction = transform.TransformDirection(new Vector3(x, 0.0f, y));
                var ray = new Ray(transform.position, direction);

                if (Physics.Raycast(ray, out var hit, gun.Current.Radius, LayerMask.GetMask("Environment", "Entity")))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity"))
                    {
                        if (inRange.Add(hit.collider)) onTargetEntry?.Invoke(hit.collider);
                        cache.Add(hit.collider);

                        point = new Vector3(x * gun.Current.Radius, 0.0f, y * gun.Current.Radius);
                    }
                    else point = transform.InverseTransformPoint(hit.point);
                }
                else point = new Vector3(x * gun.Current.Radius, 0.0f, y * gun.Current.Radius);
                
                var index = i + 1;
                vertices[index] = point;
                UVs[index] = new Vector2(x,y);
                normals[index] = Vector3.up;
            }
            
            foreach (var target in inRange)
            {
                if (cache.Contains(target)) continue;
                onTargetLeft?.Invoke(target);
            }
            inRange.IntersectWith(cache);

            var mesh = new Mesh();
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = UVs;
            mesh.triangles = triangles;

            filter.mesh = mesh;
        }
        
        private int ComputeDefinition() => Mathf.RoundToInt(gun.Current.Spread * quality);
    }
}