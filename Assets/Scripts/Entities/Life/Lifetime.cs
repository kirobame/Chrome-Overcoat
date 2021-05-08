using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flux.Event;
using Flux.Feedbacks;
using UnityEngine;

namespace Chrome
{
    public class Lifetime : MonoBehaviour, IInstaller, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        void IInjectable.OnInjectionDone(IRoot source)
        {
            root = source;
            source.onAttachment += OnRootAttachment;
            source.onDetachment += OnRootDetachment;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private IRoot root;
        
        public IIdentity Identity => identity.Value;
        private IValue<IIdentity> identity;
        
        private List<ILifebound> bounds;
        private List<IListener<Lifetime>> listeners;
        
        private Lifetime parent;
        private List<Lifetime> children;

        private int countdown;

        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
            
            children = new List<Lifetime>();
            
            bounds = transform.Fetch<ILifebound,Lifetime>().ToList();
            foreach (var lifebound in bounds) lifebound.onDestruction += RemoveBound;
            
            listeners = transform.Fetch<IListener<Lifetime>,Lifetime>().ToList();
            foreach (var listener in listeners) listener.onDestruction += RemoveListener;
        }
        void Start() => TryFetchParent();
        
        void OnDestroy()
        {
            root.onAttachment -= OnRootAttachment;
            root.onDetachment -= OnRootDetachment;
            
            foreach (var lifebound in bounds) lifebound.onDestruction -= RemoveBound;
            foreach (var listener in listeners) listener.onDestruction -= RemoveListener;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void AddChild(Lifetime child)
        {
            if (!child.transform.IsChildOf(transform))
            {
                throw new InvalidDataException($"The Lifetime : {child} isn't a child of Lifetime : {this}!");
                return;
            }

            children.Add(child);
        }
        public void RemoveChild(Lifetime child) => children.Remove(child);
        
        private void TryFetchParent()
        {
            parent = transform.GetComponentInParent<Lifetime>();
            if (parent != null) parent.AddChild(this);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public void AddBound(ILifebound lifebound)
        {
            lifebound.onDestruction += RemoveBound;
            bounds.Add(lifebound);
        }
        public void RemoveBound(ILifebound lifebound)
        {
            bounds.Remove(lifebound);
            lifebound.onDestruction -= RemoveBound;
        }
        
        public void AddListener(IListener<Lifetime> listener)
        {
            listener.onDestruction += RemoveListener;
            listeners.Add(listener);
        }
        public void RemoveListener(IListener<Lifetime> listener)
        {
            listeners.Remove(listener);
            listener.onDestruction -= RemoveListener;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public void Begin()
        {
            var args = new WrapperArgs<byte>(0);
            CallListeners(args, CheckForSpawn);
        }
        
        public void End()
        {
            foreach (var lifebound in bounds)
            {
                if (!lifebound.IsActive) continue;
                lifebound.Shutdown();
            }
            foreach (var child in children) child.End();
            
            var args = new WrapperArgs<byte>(1);
            CallListeners(args, CheckForDeath);
        }

        private void CallListeners(EventArgs args, Action<Token> callback)
        {
            countdown = 0;
            foreach (var listener in listeners)
            {
                if (!listener.IsListeningTo(args)) continue;

                var token = new Token();
                token.onConsumption += callback;

                countdown++;
                listener.Execute(token);
            }
        }
        
        private void CheckForSpawn(Token token)
        {
            token.onConsumption -= CheckForSpawn;
            
            countdown--;
            if (countdown > 0) return;
            
            foreach (var lifebound in bounds)
            {
                if (!lifebound.IsActive) continue;
                lifebound.Bootup();
            }
            foreach (var child in children) child.Begin();
        }
        private void CheckForDeath(Token token)
        {
            token.onConsumption -= CheckForDeath;
            
            countdown--;
            if (countdown > 0) return;
            
            gameObject.SetActive(false);
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void OnRootAttachment(IRoot root) => TryFetchParent();
        void OnRootDetachment(IRoot root)
        {
            if (parent != null) parent.RemoveChild(this);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 0;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set<Lifetime>(this);
    }
}