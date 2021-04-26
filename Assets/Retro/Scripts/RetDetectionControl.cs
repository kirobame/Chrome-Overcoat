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
        public event Action<IEnumerable<RetTarget>, RetTarget> onTargetEntry;
        public event Action<IEnumerable<RetTarget>, RetTarget> onTargetLeft;

        public IEnumerable<RetTarget> Targets => inRange;
        
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterBody body;
        [FoldoutGroup("Dependencies"), SerializeField] private MeshFilter filter;
        [FoldoutGroup("Dependencies"), SerializeField] private new MeshRenderer renderer;

        [FoldoutGroup("Values"), SerializeField] private LayerMask mask;
        [FoldoutGroup("Values"), SerializeField] private float spread;
        [FoldoutGroup("Values"), SerializeField] private float radius;
        [FoldoutGroup("Values"), Min(3), SerializeField] private int definition;

        private HashSet<RetTarget> inRange;
        private HashSet<RetTarget> outOfRange;
        
        private Vector3[] vertices;
        private Vector2[] UVs;
        private Vector3[] normals;
        private int[] triangles;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Awake()
        {
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
        }
        void Start()
        {
            inRange = new HashSet<RetTarget>();
            outOfRange = new HashSet<RetTarget>();

            foreach (var target in Repository.GetAll<RetTarget>(RetReference.Targets)) outOfRange.Add(target);

            Events.Subscribe<RetTarget>(RetEvent.OnTargetSpawn, OnTargetSpawn);
            Events.Subscribe<RetTarget>(RetEvent.OnTargetDeath, OnTargetDeath);
        }
        
        void OnDestroy()
        {
            Events.Unsubscribe<RetTarget>(RetEvent.OnTargetSpawn, OnTargetSpawn);
            Events.Unsubscribe<RetTarget>(RetEvent.OnTargetDeath, OnTargetDeath);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (!body.IsGrounded)
            {
                if (inRange.Any())
                {
                    foreach (var target in inRange)
                    {
                        outOfRange.Add(target);
                        onTargetLeft?.Invoke(inRange, target);
                    }
                    inRange.Clear();
                }
                
                renderer.enabled = false;
                return;
            }
            else renderer.enabled = true;
            
            var baseAngle = -spread * 0.5f + 90.0f;
            var step = spread / definition;
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

                if (Physics.Raycast(ray, out var hit, radius, mask)) point = transform.InverseTransformPoint(hit.point);
                else point = new Vector3(x * radius, 0.0f, y * radius);

                var index = i + 1;
                vertices[index] = point;
                UVs[index] = new Vector2(x,y);
                normals[index] = Vector3.up;
            }

            var mesh = filter.mesh;
            mesh.Clear();
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = UVs;
            mesh.triangles = triangles;

            foreach (var target in Repository.GetAll<RetTarget>(RetReference.Targets))
            {
                // TO DO -> Add angle check !
                var distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < radius)
                {
                    var center = target.Collider.bounds.center;
                    var displacement = center - transform.position;
                    var ray = new Ray(center, displacement.normalized);

                    if (!Physics.Raycast(ray, displacement.magnitude, mask)) HandleInRangeTarget(target);
                    else HandleOutOfRangeTarget(target);
                }
                else HandleOutOfRangeTarget(target);
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        private void HandleInRangeTarget(RetTarget target)
        {
            if (outOfRange.Contains(target))
            {
                outOfRange.Remove(target);
                inRange.Add(target);
                
                onTargetEntry?.Invoke(inRange, target);
            }
        }
        private void HandleOutOfRangeTarget(RetTarget target)
        {
            if (inRange.Contains(target))
            {
                inRange.Remove(target);
                outOfRange.Add(target);
                        
                onTargetLeft?.Invoke(inRange, target);
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnTargetSpawn(RetTarget target) => outOfRange.Add(target);
        void OnTargetDeath(RetTarget target)
        {
            if (inRange.Remove(target)) onTargetLeft?.Invoke(inRange, target);
            else outOfRange.Remove(target);
        }
    }
}