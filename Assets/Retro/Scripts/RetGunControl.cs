using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunControl : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private RetDetectionControl detection;

        private Coroutine routine;
        
        void Awake() => detection.onTargetEntry += OnTargetEntry;
        void OnDestroy() => detection.onTargetEntry -= OnTargetEntry;

        private IEnumerator Routine(RetTarget target)
        {
            Debug.Log($"Handling : {target}");
            yield return new WaitForSeconds(1.25f);
            
            Debug.Log($"Finished handling : {target}");
            if (detection.Targets.Any()) StartCoroutine(Routine(detection.Targets.First()));
            else routine = null;
        }

        void OnTargetEntry(IEnumerable<RetTarget> targets, RetTarget target)
        {
            if (routine != null) return;
            routine = StartCoroutine(Routine(target));
        }
    }
}