using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public class FlagTranslator
    {
        public FlagTranslator() => lookups = new List<(Type type, ushort offset)>();

        private List<(Type type, ushort offset)> lookups;
        
        public int Translate(Enum flag)
        {
            byte value;
            
            try { value = Convert.ToByte(flag); }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                return 0;
            }
            
            var type = flag.GetType();
            var index = -1;

            for (var i = 0; i < lookups.Count; i++)
            {
                if (lookups[i].type != type) continue;

                index = i;
                break;
            }

            if (index == -1)
            {
                var offset = (ushort)lookups.Count;
                lookups.Add((type, offset));

                index = lookups.Count - 1;
            }

            return int.MinValue + lookups[index].offset * 255 + value;
        }
        public Enum Inverse(int value)
        {
            uint conversion = value < 0 ? (uint)(value - int.MinValue) : (uint)value;
            var index = (uint)Math.Floor(conversion / 255.0f);
            var flag = conversion - index * 255;

            return (Enum)Enum.ToObject(lookups[(int)index].type, (byte)flag);
        }
        
        public void Reset() => lookups.Clear();
    }
}