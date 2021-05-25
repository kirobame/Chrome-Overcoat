using System;
using System.Collections.Generic;
using System.Linq;
using Flux;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Chrome
{
    public class InputHandler : MonoBehaviour, IInstaller
    {
        #region Nested Types

        private interface IContinuous
        {
            bool IsOn { get; }

            void Update();
        }
        
        private abstract class Entry
        {
            public Entry(Object source, InputAction action)
            {
                Source = source.GetInstanceID();
                Id = Guid.NewGuid();
                isActive = true;

                this.action = action;

                action.started += Start;
                action.performed += Perform;
                action.canceled += Cancel;
            }
            
            public int Source { get; set; }
            public Guid Id { get; private set; }
            public InputAction Action => action;

            public bool IsOn => isActive && !isLocked;
            public bool isActive;
            public bool isLocked;

            protected InputAction action;

            public void Unbind()
            {
                action.started -= Start;
                action.performed -= Perform;
                action.canceled -= Cancel;
            }

            private void Start(InputAction.CallbackContext context)
            {
                if (!IsOn) return;
                OnStarted(context);
            }
            protected virtual void OnStarted(InputAction.CallbackContext context) { }

            private void Perform(InputAction.CallbackContext context)
            {
                if (!IsOn) return;
                OnPerformed(context);
            }
            protected virtual void OnPerformed(InputAction.CallbackContext context) { }
            
            private void Cancel(InputAction.CallbackContext context)
            {
                if (!IsOn) return;
                OnCancelled(context);
            }
            protected virtual void OnCancelled(InputAction.CallbackContext context) { }
        }

        private class StandardEntry : Entry
        {
            public StandardEntry(Object source, InputAction action, Action<InputAction.CallbackContext, InputCallbackType> method) : base(source, action)
            {
                this.method = method;
            }

            private Action<InputAction.CallbackContext, InputCallbackType> method;

            protected override void OnStarted(InputAction.CallbackContext context) => method(context, InputCallbackType.Started);
            protected override void OnPerformed(InputAction.CallbackContext context) => method(context, InputCallbackType.Performed);
            protected override void OnCancelled(InputAction.CallbackContext context) => method(context, InputCallbackType.Cancelled);
        }
        
        private class KeyEntry : Entry, IContinuous
        {
            public KeyEntry(Object source, InputAction action, CachedValue<Key> key) : base(source, action)
            {
                callbackType = InputCallbackType.Ended;
                this.key = key;
            }

            private InputCallbackType callbackType;
            private CachedValue<Key> key;

            public void Update()
            {
                if (callbackType == InputCallbackType.Started)
                {
                    key.Value = Key.Active;
                    callbackType = InputCallbackType.Performed;
                } 
                if (callbackType == InputCallbackType.Cancelled) key.Value = Key.Inactive;
            }

            protected override void OnStarted(InputAction.CallbackContext context)
            {
                key.Value = Key.Down;
                callbackType = InputCallbackType.Started;
            }
            protected override void OnCancelled(InputAction.CallbackContext context)
            {
                key.Value = Key.Up;
                callbackType = InputCallbackType.Cancelled;
            }
        }

        private class ValueEntry<T> : Entry where T : struct
        {
            public ValueEntry(Object source, InputAction action, CachedValue<T> link) : base(source, action)
            {
                this.link = link;
            }
            
            private CachedValue<T> link;

            protected override void OnStarted(InputAction.CallbackContext context) => link.Value = context.ReadValue<T>();
            protected override void OnPerformed(InputAction.CallbackContext context) => link.Value = context.ReadValue<T>();
            protected override void OnCancelled(InputAction.CallbackContext context) => link.Value = context.ReadValue<T>();
        }
        
        #endregion

        private Dictionary<InputAction, List<Entry>> repository;
        private List<IContinuous> callbackListeners;

        private bool hasBeenBootedUp => repository != null;

        //--------------------------------------------------------------------------------------------------------------/

        void Bootup()
        {
            repository = new Dictionary<InputAction, List<Entry>>();
            callbackListeners = new List<IContinuous>();
        }
        void OnDestroy()
        {
            foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.Unbind();
            repository.Clear();
        }

        void OnEnable()
        {
            if (!hasBeenBootedUp) Bootup();
            foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.isLocked = false;
        }
        void OnDisable() { foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.isLocked = true; }

        void LateUpdate()
        {
            foreach (var callbackListener in callbackListeners)
            {
                if (!callbackListener.IsOn) continue;
                callbackListener.Update();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public Guid Bind(string reference, Object origin, Action<InputAction.CallbackContext, InputCallbackType> method)
        {
            var action = GetAction(reference);
            return AddEntry(new StandardEntry(origin, action, method));
        }
        public Guid BindKey(string reference, Object origin, CachedValue<Key> key)
        {
            var action = GetAction(reference);
            return AddEntry(new KeyEntry(origin, action, key));
        }
        public Guid BindValue<T>(string reference, Object origin, CachedValue<T> link) where T : struct
        {
            var action = GetAction(reference);
            return AddEntry(new ValueEntry<T>(origin, action, link));
        }
        
        public void UnbindAll(string reference, Object origin)
        {
            var action = GetAction(reference);
            var source = origin.GetInstanceID();

            var list = repository[action];
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Source != source) continue;
                
                RemoveEntry(list, i);
                i--;
            }
            list.RemoveAll(entry => entry.Source == source);
        }
        public void Unbind(string reference, Object key, Guid id)
        {
            var action = GetAction(reference);
            var source = key.GetInstanceID();
            
            var list = repository[action];
            var index = list.FindIndex(entry => entry.Source == source && entry.Id == id);
            RemoveEntry(list, index);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void SetActiveAll(Object key, bool value)
        {
            var source = key.GetInstanceID();
            foreach (var entry in repository.SelectMany(kvp => kvp.Value))
            {
                if (entry.Source != source) continue;
                entry.isActive = value;
            }
        }
        public void SetActive(string reference, Object key, bool value)
        {
            var action = GetAction(reference);
            var source = key.GetInstanceID();

            var entry = repository[action].Find(candidate => candidate.Source == source);
            entry.isActive = value;
        }

        //--------------------------------------------------------------------------------------------------------------/

        private InputAction GetAction(string reference)
        {
            var board = Blackboard.Global.Get<IBlackboard>(InputRefs.BOARD);
            return board.Get<InputAction>(reference);
        }

        private Guid AddEntry(Entry entry)
        {
            entry.isLocked = !enabled;
            if (entry is IContinuous continuous) callbackListeners.Add(continuous);

            if (repository.TryGetValue(entry.Action, out var list)) list.Add(entry);
            else repository.Add(entry.Action, new List<Entry>() { entry });

            return entry.Id;
        }
        private void RemoveEntry(List<Entry> entries, int index)
        {
            if (entries[index] is IContinuous continuous) callbackListeners.Remove(continuous);
                                
            entries[index].Unbind();
            entries.RemoveAt(index);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet)
        {
            if (!hasBeenBootedUp) Bootup();
            packet.Set(this);
        }
    }
}