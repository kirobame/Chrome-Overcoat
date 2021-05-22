using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Chrome
{
    [RequireComponent(typeof(BoxCollider))]
    public class CoverZone : MonoBehaviour, IInjectable, IInjectionCallbackListener
    {
        private const float MARGIN = 0.45f;
        private const float SKIN_WIDTH = 0.05f;
        private const int NEIGHBOUR_SEARCH = 36;

        //--------------------------------------------------------------------------------------------------------------/

        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            area = new AnyValue<Area>();
            injections = new IValue[] { area };
        }
        
        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            foreach (var spot in spots)
            {
                spot.Area = area.Value;
                CoverSystem.Register(spot);
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        [SerializeField] private float radius;
        [SerializeField] private float spacing;
        [SerializeField] private float offset;
        
        [Space, SerializeField] private Transform height;
        [SerializeField, ReadOnly] private List<CoverSpot> spots = new List<CoverSpot>();

        private float diameter => radius * 2.0f;
        private float step => diameter + spacing;

        private IValue<Area> area;

        //--------------------------------------------------------------------------------------------------------------/

        #if UNITY_EDITOR
        [Button]
        private void Bake()
        {
            spots.Clear();

            var mask = LayerMask.GetMask("Environment");
            var collider = GetComponent<BoxCollider>();
            
            var start = collider.bounds.min + Vector3.right * radius;
            var count = Mathf.RoundToInt(collider.bounds.size.x / step);
            
            for (var i = 0; i < count; i++)
            {
                var ray = new Ray(start + Vector3.right * (i * step), Vector3.forward);
                var hits = Physics.RaycastAll(ray, collider.bounds.size.z, mask);

                TryRegister(hits, mask);

                var endRay = new Ray(ray.GetPoint(collider.bounds.size.z), Vector3.back);
                hits = Physics.RaycastAll(endRay, collider.bounds.size.z, mask);
                
                TryRegister(hits, mask);
            }
            
            start = collider.bounds.min + Vector3.forward * radius;
            count = Mathf.RoundToInt(collider.bounds.size.z / step);
            
            for (var i = 0; i < count; i++)
            {
                var ray = new Ray(start + Vector3.forward * (i * step), Vector3.right);
                var hits = Physics.RaycastAll(ray, collider.bounds.size.x, mask);

                TryRegister(hits, mask);
                
                var endRay = new Ray(ray.GetPoint(collider.bounds.size.x), Vector3.left);
                hits = Physics.RaycastAll(endRay, collider.bounds.size.x, mask);
                
                TryRegister(hits, mask);
            }

            for (var i = 0; i < spots.Count; i++)
            {
                spots.RemoveAll(cover =>
                {
                    if (cover == spots[i]) return false;

                    var position = cover.Position;
                    position.y = spots[i].Position.y;

                    var distance = Vector3.Distance(spots[i].Position, position);
                    return distance <= diameter;
                });
            }
        }

        private void TryRegister(RaycastHit[] hits, LayerMask mask)
        {
            foreach (var hit in hits)
            {
                if (Mathf.Abs(hit.normal.y) > 0.25f) continue;
                
                var canBeAdded = true;
                var point = hit.point + hit.normal * (radius + SKIN_WIDTH + offset);

                for (var i = 0; i < NEIGHBOUR_SEARCH; i++)
                {
                    var angle = i / (float)NEIGHBOUR_SEARCH * 360.0f * Mathf.Deg2Rad;
                    var direction = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sign(angle));

                    if (Physics.Raycast(point, direction, radius, mask))
                    {
                        canBeAdded = false;
                        break;
                    }
                }
                
                if (!canBeAdded) continue;

                point.y = height.position.y;
                if (!NavMesh.SamplePosition(point, out var navHit, MARGIN, NavMesh.AllAreas)) continue;
                
                var spot = new CoverSpot(navHit.position + Vector3.up * SKIN_WIDTH, -hit.normal);
                spots.Add(spot);
            }
        }
        
        void OnDrawGizmos()
        {
            var selectedGameObject = UnityEditor.Selection.activeGameObject;
            if (selectedGameObject == null || selectedGameObject != gameObject && !transform.IsChildOf(selectedGameObject.transform) && selectedGameObject.transform != transform.parent) return;

            var discColor = Color.magenta;
            discColor.a = 0.25f;

            var discOutlineColor = Color.black;
            discOutlineColor.a = 0.75f;

            UnityEditor.Handles.zTest = CompareFunction.Less;
            foreach (var spot in spots)
            {
                UnityEditor.Handles.color = discColor;
                UnityEditor.Handles.DrawSolidDisc(spot.Position, Vector3.up, radius);

                UnityEditor.Handles.color = discOutlineColor;
                UnityEditor.Handles.DrawWireDisc(spot.Position, Vector3.up, radius, 0.1f);
                
                UnityEditor.Handles.ArrowHandleCap(0, spot.Position, Quaternion.LookRotation(spot.Orientation), radius, EventType.Repaint);
                UnityEditor.Handles.SphereHandleCap(0, spot.Position, Quaternion.identity, 0.1f, EventType.Repaint);
            }
        }
        #endif
    }
}