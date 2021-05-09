using System;
using System.Collections.Generic;
using System.Linq;
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

            private InputAction action;
            private Action<InputAction.CallbackContext, InputCallbackType> method;

            public void Unbind()
            {
                action.started -= OnStarted;
                action.performed -= OnPerformed;
                action.canceled -= OnCancelled;
            }
            
            private void OnStarted(InputAction.CallbackContext context)
            {
                if (!isActive || isLocked) return;
                method(context, InputCallbackType.Started);
            }
            private void OnPerformed(InputAction.CallbackContext context)
            {
                if (!isActive || isLocked) return;
                method(context, InputCallbackType.Started);
            }
            private void OnCancelled(InputAction.CallbackContext context)
            {
                if (!isActive || isLocked) return;
                method(context, InputCallbackType.Started);
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
        
        void OnEnable() { foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.isLocked = true; }
        void OnDisable() { foreach (var entry in repository.SelectMany(kvp => kvp.Value)) entry.isLocked = false; }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Bind(string reference, Object key, Action<InputAction.CallbackContext, InputCallbackType> method)
        {
            var action = GetAction(reference);
            
            var entry = new Entry(key, action, method);
            entry.isLocked = enabled;

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