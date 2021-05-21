using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct Cadence
    {
        public Cadence(float value)
        {
            this.value = value;
            timer = 0.0f;
        }

        [SerializeField] private float value;

        private float timer;

        public void Reset() => timer = 0.0f;

        public bool Update(float deltaTime)
        {
            timer += Time.deltaTime;
            if (timer >= value)
            {
                timer -= value;
                return true;
            }
            else return false;
        }
    }
}