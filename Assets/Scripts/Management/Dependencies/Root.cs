using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chrome
{
    public abstract class Root : MonoBehaviour, IRoot
    {
        public event Action<IRoot> onAttachment;
        public event Action<IRoot> onDetachment;
        
        public IRoot Parent { get; private set; }

        public string Tag => tag.ToString();
        [SerializeField] private new RootType tag;

        public Transform Transform => transform;
        public Packet Packet { get; private set; }
        
        public IReadOnlyList<IRoot> Children => children;
        private List<IRoot> children;

        //--------------------------------------------------------------------------------------------------------------/

        protected virtual void Awake()
        {
            children = new List<IRoot>();
            
            Packet = new Packet();
            Packet.Set<IRoot>(this);
            HandlePacket(Packet);

            var installers = transform.Fetch<IInstaller,IRoot>();
            Array.Sort(installers, (first, second) => first.Priority.CompareTo(second.Priority));
            foreach (var installer in installers) installer.InstallDependenciesOn(Packet);
            
            TryFetchParent();
            InjectDependencies();
        }
        void OnDestroy() => onDetachment?.Invoke(this);
        
        protected virtual void HandlePacket(Packet packet) { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public void AttachTo(Transform parent)
        {
            onDetachment?.Invoke(this);
            transform.SetParent(parent);

            TryFetchParent();
        }
        public void AddChild(IRoot child)
        {
            if (!child.Transform.IsChildOf(transform))
            {
                throw new InvalidDataException($"The IRoot : {child} isn't a child of Root : {this}!");
                return;
            }
            
            child.onDetachment += OnChildDetachment;
            children.Add(child);
        }

        //--------------------------------------------------------------------------------------------------------------/

        private void TryFetchParent()
        {
            if (transform.parent == null)
            {
                Parent = null;
                return;
            }
            
            Parent = transform.parent.GetComponentInParent<IRoot>();
            if (Parent != null)
            {
                Parent.AddChild(this);
                onAttachment?.Invoke(this);
            }
        }

        private void InjectDependencies()
        {
            foreach (var child in children) child.onDetachment -= OnChildDetachment;
            children.Clear();
            
            InjectDependenciesOn(transform);
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                InjectDependenciesFrom(child);
            }
        }
        private void InjectDependenciesFrom(Transform transform)
        {
            if (transform.HasComponent<IRoot>()) return;

            InjectDependenciesOn(transform);
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                InjectDependenciesFrom(child);
            }
        }
        private void InjectDependenciesOn(Transform transform)
        {
            foreach (var injectable in transform.GetComponents<IInjectable>())
            {
                injectable.PrepareInjection();
                foreach (var injection in injectable.Injections) injection.FillIn(Packet);
                
                if (injectable is IInjectionCallbackListener callbackListener) callbackListener.OnInjectionDone(this);
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnChildDetachment(IRoot child)
        {
            child.onDetachment -= OnChildDetachment;
            children.Remove(child);
        }
    }
}