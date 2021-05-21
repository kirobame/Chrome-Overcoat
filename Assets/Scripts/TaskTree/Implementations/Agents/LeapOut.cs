using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class LeapOut : TaskNode
    {
        #region Nested types

        private struct LeapData
        {
            public LeapData(float time, Vector3 velocity, Vector3 start, Vector3 destination, Vector3 highestAvailablePosition)
            {
                this.time = time;
                this.velocity = velocity;
                this.destination = destination;
                this.highestAvailablePosition = highestAvailablePosition;
                this.start =start;
            }
            
            public float time;
            public Vector3 velocity;

            public Vector3 start;
            public Vector3 destination;
            public Vector3 highestAvailablePosition;
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------/

        public LeapOut(float speed, float range, float height, int resolution, IValue<NavMeshAgent> nav, IValue<Collider> collider, IValue<Transform> from, IValue<Transform> aim)
        {
            this.speed = speed;
            this.range = range;
            this.height = height;
            this.resolution = resolution;

            this.nav = nav;
            this.collider = collider;
            this.from = from;
            this.aim = aim;
        }
        
        private float speed;
        private float range;
        private float height;
        private int resolution;
        
        private IValue<NavMeshAgent> nav;
        private IValue<Collider> collider;
        private IValue<Transform> from;
        private IValue<Transform> aim;

        private float timer;
        private bool canLeap;
        private LeapData data;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void OnPrepare(Packet packet)
        {
            if (!nav.IsValid(packet) || !collider.IsValid(packet) || !from.IsValid(packet)) return;

            timer = 0.0f;
            var gravity = Physics.gravity.y;

            data = ComputeData(gravity);
            canLeap = true;
            
            var previous =  data.start + Vector3.up * 0.1f;
            for (var i = 1; i <= resolution; i++)
            {
                var simulatedTime = i / (float)resolution * data.time;
                var advance = data.velocity * simulatedTime + Vector3.up * (gravity * simulatedTime * simulatedTime / 2.0f);

                var current =  data.start + advance + Vector3.up * 0.1f;
                var displacement = current - previous;
                
                if (i < resolution && collider.Value.CastFrom(current, displacement, LayerMask.GetMask("Environment")))
                {
                    canLeap = false;
                    break;
                }
                
                previous = current;
            }

            if (!canLeap)
            { 
                NavMesh.SamplePosition(data.destination, out var navHit, 50.0f, NavMesh.AllAreas);
                
                nav.Value.updateRotation = true;
                nav.Value.isStopped = false;
                
                nav.Value.SetDestination(navHit.position);
                
                if (aim.IsValid(packet)) aim.Value.localRotation = Quaternion.identity;
            }
            else
            {
                nav.Value.updateRotation = false;
                nav.Value.enabled = false;
            }
        }

        private Vector3 GetDestination(Vector3 selfPosition)
        {
            var Y = from.Value.position.y;
            selfPosition.y = Y;

            var direction = Vector3.Normalize(selfPosition - from.Value.position);
            return selfPosition + direction * range;
        }

        private Vector3 GetHighestPointFromDestination(Vector3 destination)
        {
            var ray = new Ray(destination + Vector3.up * 100.0f, Vector3.down);
            if (Physics.Raycast(ray, out var hit, 100.0f, LayerMask.GetMask("Environment"))) destination = hit.point;

            if (!NavMesh.SamplePosition(destination, out var navHit, 50.0f, NavMesh.AllAreas)) throw new InvalidOperationException("No available point to flee to can be found !");
            return navHit.position;
        }

        private LeapData ComputeData(float gravity)
        {
            var selfPosition = nav.Value.transform.position;

            var destination = GetDestination(selfPosition);
            var highestAvailablePosition = GetHighestPointFromDestination(destination);
            
            var totalAdvanceY = highestAvailablePosition.y - selfPosition.y;
            var usedHeight = totalAdvanceY >= 0 ? totalAdvanceY + height : height;
            
            var time = Mathf.Sqrt(-2 * usedHeight/ gravity) + Mathf.Sqrt(2 * (totalAdvanceY - usedHeight) / gravity);
            var totalAdvanceXZ = new Vector3(highestAvailablePosition.x - selfPosition.x, 0.0f, highestAvailablePosition.z - selfPosition.z);
            var velocity = totalAdvanceXZ / time + Vector3.up * (Mathf.Sqrt(-2.0f * gravity * usedHeight) * -Mathf.Sign(gravity));

            return new LeapData(time, velocity, collider.Value.transform.position, destination, highestAvailablePosition);
        }

        //--------------------------------------------------------------------------------------------------------------/

        protected override void OnUse(Packet packet)
        {
            if (!nav.IsValid(packet) || !collider.IsValid(packet) || !from.IsValid(packet) || !aim.IsValid(packet))
            {
                isDone = true;
                return;
            }

            if (canLeap)
            {
                timer += Time.deltaTime * speed;

                if (timer >= data.time)
                {
                    nav.Value.enabled = true;
                    nav.Value.Warp(data.highestAvailablePosition);
                    
                    isDone = true;
                }
                else
                {
                    var advance = data.velocity * timer + Vector3.up * (Physics.gravity.y * timer * timer / 2.0f);

                    var end = data.start + advance;
                    var direction = Vector3.Normalize(end - collider.Value.transform.position);

                    aim.Value.rotation = Quaternion.LookRotation(direction);
                    collider.Value.transform.position = end;
                }
            }
            else if (nav.Value.hasPath && nav.Value.remainingDistance <= nav.Value.stoppingDistance + 0.1f)  isDone = true;
        }

        protected override void OnShutdown(Packet packet)
        {
            nav.Value.enabled = true;
            nav.Value.Warp(nav.Value.transform.position);
        }
    }
}