using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunModel : MonoBehaviour
    {
        public Transform FireAnchor => fireAnchor;
        
        [FoldoutGroup("Dependencies"), SerializeField] private Transform fireAnchor;

        public void Discard()
        {
            var children = new Transform[fireAnchor.childCount];
            for (var i = 0; i < children.Length; i++) children[i] = fireAnchor.GetChild(i);
            
            for (var i = 0; i < children.Length; i++)
            {
                children[i].SetParent(null);
                children[i].gameObject.SetActive(false);
            }
            
            Destroy(gameObject);
        }
    }
}