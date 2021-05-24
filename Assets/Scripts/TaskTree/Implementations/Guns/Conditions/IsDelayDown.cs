using System;
using System.Collections;
using Flux;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class IsDelayDown : Condition
    {
        public IsDelayDown(float delay) => this.delay = delay;

        private float delay;
        private Coroutine routine;
        
        public override bool Check(Packet packet)
        {
            if (routine == null)
            {
                routine = Routines.Start(Routine());
                return true;
            }
            else return false;
        }

        private IEnumerator Routine()
        {
            yield return new WaitForSeconds(delay);
            routine = null;
        }
    }
}