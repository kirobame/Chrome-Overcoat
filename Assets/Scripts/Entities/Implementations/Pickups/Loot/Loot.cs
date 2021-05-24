using System;
using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chrome
{
    public abstract class Loot : MonoBehaviour, IPickable, ILifebound, IInjectable, IInstaller, IListener<Lifetime>
    {
        protected const int GRAPH_LAYER = 0;
        protected const int INDICATOR_LAYER = 1;

        protected const string VOID_TAG = "Void";

        protected const string IS_ACTIVE = "IsActive";
        protected const string IS_INDICATING = "IsIndicating";
        
        //--------------------------------------------------------------------------------------------------------------/
        
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        void IInjectable.PrepareInjection()
        {
            animator = new AnyValue<Animator>();
            identity = new AnyValue<IIdentity>();
            lifetime = new AnyValue<Lifetime>();
            
            injections = new List<IValue>()
            {
                identity,
                animator,
                lifetime
            };
            
            PrepareInjection();
        }
        protected virtual void PrepareInjection() { }
        
        //--------------------------------------------------------------------------------------------------------------/

        #region Destruction events

        private event Action<ILifebound> onLifeboundDestruction;
        event Action<ILifebound> IActive<ILifebound>.onDestruction
        {
            add => onLifeboundDestruction += value;
            remove => onLifeboundDestruction -= value;
        }

        private event Action<IInteraction> onInteractionDestruction;
        event Action<IInteraction> IActive<IInteraction>.onDestruction
        {
            add => onInteractionDestruction += value;
            remove => onInteractionDestruction -= value;
        }
        
        private event Action<IListener<Lifetime>> onListenerDestruction;
        event Action<IListener<Lifetime>> IActive<IListener<Lifetime>>.onDestruction
        {
            add => onListenerDestruction += value;
            remove => onListenerDestruction -= value;
        }

        #endregion
        
        public Transform Transform => transform;
        public float Radius => radius;
        public bool IsActive => true;

        [FoldoutGroup("Values"), SerializeField] private float radius;

        protected IValue<Animator> animator;
        protected IValue<Lifetime> lifetime;
        protected IValue<IIdentity> identity;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected virtual void OnDestroy()
        {
            onLifeboundDestruction?.Invoke(this);
            onInteractionDestruction?.Invoke(this);
            onListenerDestruction?.Invoke(this);
        }

        public virtual void Bootup(byte code)
        {
            Repository.AddTo(Reference.Pickups, (IPickable)this);
            animator.Value.SetBool(IS_ACTIVE, true);
        }
        public virtual void Shutdown(byte code) => Repository.RemoveFrom(Reference.Pickups, (IPickable)this);

        public virtual void OnHoverStart(IIdentity source) => animator.Value.SetBool(IS_INDICATING, true);
        public virtual void OnHoverEnd(IIdentity source) => animator.Value.SetBool(IS_INDICATING, false);
        
        public void Pickup(IIdentity source)
        {
            OnHoverEnd(source);
            OnPickup(source);
            
            lifetime.Value.End();
        }
        protected abstract void OnPickup(IIdentity source);

        //--------------------------------------------------------------------------------------------------------------/

        public bool IsListeningTo(EventArgs args) => Lifetime.IsShutdownMessage(args);
        public void Execute(Token token) => StartCoroutine(WaitForAnimations(token));

        private IEnumerator WaitForAnimations(Token token)
        {
            animator.Value.SetBool(IS_ACTIVE, false);

            var graphStateInfo = animator.Value.GetCurrentAnimatorStateInfo(GRAPH_LAYER);
            while (!graphStateInfo.IsTag(VOID_TAG))
            {
                yield return new WaitForEndOfFrame();
                graphStateInfo = animator.Value.GetCurrentAnimatorStateInfo(GRAPH_LAYER);
            }
            
            var indicatorStateInfo = animator.Value.GetCurrentAnimatorStateInfo(INDICATOR_LAYER);
            while (!indicatorStateInfo.IsTag(VOID_TAG))
            {
                yield return new WaitForEndOfFrame();
                indicatorStateInfo = animator.Value.GetCurrentAnimatorStateInfo(INDICATOR_LAYER);
            }
            
            token.Consume();
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}