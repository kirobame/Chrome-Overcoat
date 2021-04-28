using System;
using System.Collections.Generic;
using Flux.Event;
using Flux.Feedbacks;
using UnityEngine;

namespace Chrome
{
    public class Lifetime : MonoBehaviour
    {
        [SerializeField] private Sequence sequence;

        private SendbackArgs args;
        private bool hasBeenBootedUp;
        
        //private ILifebound[] lifebounds;
        private List<ILifebound> lifeboundsList = new List<ILifebound>();

        void Awake()
        {
            hasBeenBootedUp = false;
            
            args = new SendbackArgs();
            args.onDone += OnSequenceDone;

            //lifebounds = transform.root.GetComponentsInChildren<ILifebound>();

            GetILifebounds(transform.root);
        }

        void GetILifebounds(Transform tr)
        {
            //if (tr.GetComponent<Lifetime>() != null && tr.GetComponent<Lifetime>() != this) return;

            ILifebound[] lifebounds = tr.GetComponents<ILifebound>();

            foreach (var lifebound in lifebounds)
                if (lifebound != null && !lifeboundsList.Contains(lifebound))
                    lifeboundsList.Add(lifebound);

            if (tr.childCount > 0)
                foreach (Transform child in tr)
                    GetILifebounds(child);
        }
        
        void OnEnable()
        {
            if (!hasBeenBootedUp)
            {
                hasBeenBootedUp = true;
                return;
            }
            
            foreach (var lifebound in lifeboundsList) lifebound.Bootup();
        }
        
        public void End()
        {
            foreach (var lifebound in lifeboundsList) lifebound.Shutdown();
            sequence.Play(args);
        }

        void OnSequenceDone(EventArgs args) => transform.root.gameObject.SetActive(false);
    }
}