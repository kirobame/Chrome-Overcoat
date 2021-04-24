﻿using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CachedValue<T> : IValue<T>
    {
        public CachedValue(T value) => this.value = value;
        
        public object RawValue => value;
        public T Value => value;

        [SerializeField] private T value;

        public bool IsValid(Packet packet) => true;
    }
}