using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Chrome
{
    public class Cover : MonoBehaviour, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            area = new AnyValue<Area>();
            injections = new IValue[] { area };
        }

        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            spot = new CoverSpot(transform.position, transform.forward);
            spot.Area = area.Value;
            
            CoverSystem.Register(spot);
        }

        //--------------------------------------------------------------------------------------------------------------/

        [SerializeField] private float radius = 0.5f;
        
        private IValue<Area> area;
        private CoverSpot spot;
        
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var selectedGameObject = UnityEditor.Selection.activeGameObject;
            if (selectedGameObject == null || selectedGameObject != gameObject && !transform.IsChildOf(selectedGameObject.transform) && selectedGameObject.transform != transform.parent) return;
            
            var discColor = Color.magenta;
            discColor.a = 0.25f;

            var discOutlineColor = Color.black;
            discOutlineColor.a = 0.75f;
            
            UnityEditor.Handles.zTest = CompareFunction.Less;
            
            UnityEditor.Handles.color = discColor;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, radius);

            UnityEditor.Handles.color = discOutlineColor;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius, 0.1f);
                
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward), radius, EventType.Repaint);
            UnityEditor.Handles.SphereHandleCap(0, transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
        }
        #endif
    }
}