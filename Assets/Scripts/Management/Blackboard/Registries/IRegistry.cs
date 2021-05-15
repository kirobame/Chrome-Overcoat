﻿namespace Chrome
{
    public interface IRegistry
    {
        object RawValue { get; }

        void Set(object rawValue);
    }
    public interface IRegistry<out T> : IRegistry
    {
        T Value { get; }
    }
}