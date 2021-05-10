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

        private class Entry
        {
            public Entry(Object key, InputAction action, Action<InputAction.CallbackContext, InputCallbackType> method)
            {
                Key = key.GetInstanceID();
                isActive = true;

                this.action = action;
                this.method = method;

                action.started += OnStarted;
                action.performed += OnPerformed;
                action.canceled += OnCancelled;
            }
            
            public int Key { get; set; }

            public bool isActive;
            public bool isLocked;

            protected InputAction action;
            protected Action<InputAction.CallbackContext, InputCallbackType> method;
            
            public void Unbind()
            {
                action.started -= OnStarted;
                action.performed -= OnPerformed;
                action.canceled -= OnCancelled;
            }

            protected virtual void OnStarted(InputAction.CallbackContext context)
            {
                if (!isActive || isLocked) return;
                method(context, InputCallbackType.Started);
            }
            protected virtual void OnPerformed(InputAction.CallbackContext context)
            {
                if (!isActive || isLocked) return;
                method(context, InputCallbackType.Performed);
            }
            protected virtual void OnCancelled(InputAction.CallbackContext context)
            {
                if (!isActive || isLocked) return;
                method(context, InputCallbackType.Cancelled);
            }
        }

        private class ExtendedEntry : Entry
        {
            public ExtendedEntry(Object key, InputAction action, Action<InputAction.CallbackContext, InputCallbackType> method) : base(key, action, method) { }
         
            protected InputCallbackType callbackType;
            
            public void OnFrameEnd()
            {
                if (callbackType != InputCallbackType.Cancelled) return;
                
                method(default, InputCallbackType.Ended);
                callbackType = InputCallbackType.Ended;
            }

            protected override void OnStarted(InputAction.CallbackContext context)
            {
                base.OnStarted(context);
                callbackType = InputCallbackType.Started;
            }
            protected override void OnPerformed(InputAction.CallbackContext context)
            {
                base.OnPerformed(context);
                callbackType = InputCallbackType.Performed;
            }
            protected override void OnCancelled(InputAction.CallbackContext context)
            {
                base.OnCancelled(context);
                callbackType = InputCallbackType.Cancelled;
            }
        }
        #endregion

        private Dictionary<InputAction, List<Entry>> repository;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake() => repository = new Dictionary<InputAction, List<Entry>>();
        void OnDestroy()
        {
            foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.Unbind();
            repository.Clear();
        }
        
        void OnEnable() { foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.isLocked = false; }
        void OnDisable() { foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.isLocked = true; }

        void LateUpdate()
        {
            foreach (var entry in repository.SelectMany(kvp => kvp.Value))
            {
                if (!(entry is ExtendedEntry extendedEntry)) continue;
                extendedEntry.OnFrameEnd();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public void Bind(string reference, Object key, Action<InputAction.CallbackContext, InputCallbackType> method, bool fully = false)
        {
            var action = GetAction(reference);
            
            var entry = fully ? new ExtendedEntry(key, action, method) : new Entry(key, action, method);
            entry.isLocked = !enabled;

            Debug.Log($"BINDING {action} to {method.Method.Name} in {key}");
            if (repository.TryGetValue(action, out var list)) list.Add(entry);
            else repository.Add(action, new List<Entry>() { entry });
        }
        
        public void SetActiveAll(Object key, bool value)
        {
            var id = key.GetInstanceID();
            foreach (var entry in repository.SelectMany(kvp => kvp.Value))
            {
                if (entry.Key != id) continue;
                entry.isActive = value;
            }
        }
        public void SetActive(string reference, Object key, bool value)
        {
            var action = GetAction(reference);
            var id = key.GetInstanceID();

            var entry = repository[action].Find(candidate => candidate.Key == id);
            entry.isActive = value;
        }

        private InputAction GetAction(string reference)
        {
            var board = Blackboard.Global.Get<IBlackboard>(InputRefs.BOARD);
            return board.Get<InputAction>(reference);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}