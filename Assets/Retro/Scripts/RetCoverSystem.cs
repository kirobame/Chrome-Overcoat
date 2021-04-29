using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetCoverSystem : MonoBehaviour
    {
        #region Nested Types

        private class Entry : IComparable<Entry>
        {
            public Entry(RetCover cover, float distance, float orientation)
            {
                Cover = cover;

                this.distance = distance;
                this.orientation = orientation;
            }

            public float Score => (distance + orientation) * 0.5f;

            public Vector3 Position => Cover.transform.position;
            public RetCover Cover { get; private set; }

            private float distance;
            private float orientation;
            
            public int CompareTo(Entry other) => Score.CompareTo(other.Score);
        }
        
        private class Analysis
        {
            public Analysis(RetCoverProfile profile) => this.profile = profile;
            
            public bool HasBeenComputed { get; private set; }
            
            public RetCoverProfile Profile => profile;
            private RetCoverProfile profile;
            
            public IReadOnlyList<Entry> Entries => entries;
            private List<Entry> entries = new List<Entry>();

            public void Compute(IEnumerable<RetCover> covers)
            {
                var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
                var identity = playerBoard.Get<IIdentity>(RetPlayerBoard.REF_IDENTITY);
                
                foreach (var cover in covers)
                {
                    var distance = Vector3.Distance(identity.Root.position.Flatten(), cover.transform.position.Flatten());
                    if (distance < profile.Range.x || distance > profile.Range.y) continue;
                    
                    var height = cover.transform.position.y;
                    var direction = identity.Root.position.Flatten(height) - cover.transform.position;
                    var ray = new Ray(cover.transform.position, direction.normalized);

                    if (!Physics.Raycast(ray, direction.magnitude, LayerMask.GetMask("Environment"))) continue;

                    var orientation = (Vector3.Dot(direction.normalized, cover.transform.forward) + 1.0f) * 0.5f;
                    entries.Add(new Entry(cover, (distance - profile.Range.x) / profile.Range.y, orientation));
                }

                entries.Sort();
                HasBeenComputed = true;
            }
            public Entry GetClosestFrom(Vector3 point)
            {
                var flatPoint = point.Flatten();
                
                var entry = default(Entry);
                var minDistance = float.PositiveInfinity;
                
                foreach (var watchedEntry in entries)
                {
                    var distance = Vector3.Distance(watchedEntry.Position.Flatten(), flatPoint);
                    if (distance >= minDistance) continue;
                    
                    entry = watchedEntry;
                    minDistance = distance;
                }
                
                entries.Remove(entry);
                return entry;
            }
            
            public void Reboot()
            {
                entries.Clear();
                HasBeenComputed = false;
            }
        }

        #endregion
        
        private static HashSet<RetCover> availableCovers = new HashSet<RetCover>();
        private static HashSet<RetCover> usedCovers = new HashSet<RetCover>();

        private static Dictionary<RetCoverProfile, Analysis> analyses = new Dictionary<RetCoverProfile, Analysis>();

        public static void Register(RetCover cover) => availableCovers.Add(cover);
        public static void Free(RetCover cover)
        {
            if (usedCovers.Remove(cover)) availableCovers.Add(cover);
        }

        public static bool Request(RetCoverProfile profile, Vector3 position, out RetCover cover)
        {
            if (!analyses.TryGetValue(profile, out var analysis))
            {
                analysis = new Analysis(profile);
                analyses.Add(profile, analysis);
            }
            
            if (!analysis.HasBeenComputed) analysis.Compute(availableCovers);

            if (analysis.Entries.Count == 0)
            {
                cover = null;
                return false;
            }
            
            var entry = analysis.GetClosestFrom(position);
            while (!availableCovers.Remove(entry.Cover))
            {
                if (analysis.Entries.Count == 0)
                {
                    cover = null;
                    return false;
                }
                
                entry = analysis.GetClosestFrom(position);
            }

            usedCovers.Add(entry.Cover);
            cover = entry.Cover;
            return true;
        }

        private static void Refresh()
        {
            foreach (var analysis in analyses.Values) analysis.Reboot();
        }

        void LateUpdate() => Refresh();
    }
}