﻿using System;
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

        private ILifebound[] lifebounds;
        
        void Awake()
        {
            hasBeenBootedUp = false;
            
            args = new SendbackArgs();
            args.onDone += OnSequenceDone;

            lifebounds = transform.root.GetComponentsInChildren<ILifebound>();
        }

        void OnEnable()
        {
            if (!hasBeenBootedUp)
            {
                hasBeenBootedUp = true;
                return;
            }
            
            foreach (var lifebound in lifebounds) lifebound.Bootup();
        }
        
        public void End()
        {
            foreach (var lifebound in lifebounds) lifebound.Shutdown();
            sequence.Play(args);
        }

        void OnSequenceDone(EventArgs args) => transform.root.gameObject.SetActive(false);
    }
}