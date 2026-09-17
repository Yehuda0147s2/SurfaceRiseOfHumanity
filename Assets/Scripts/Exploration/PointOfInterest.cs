using System;
using UnityEngine;
using SurfaceRiseOfHumanity.Core;

namespace SurfaceRiseOfHumanity.Exploration
{
    public sealed class PointOfInterest : MonoBehaviour
    {
        [SerializeField] private string poiId;
        [SerializeField] private string displayName;
        [SerializeField] private WorldRegionId region = WorldRegionId.GreenZone;
        [SerializeField] private float discoveryRadius = 18f;
        public event Action<PointOfInterest> Discovered;
        public bool IsDiscovered { get; private set; }
        public string PoiId => string.IsNullOrWhiteSpace(poiId) ? gameObject.name : poiId;
        public string DisplayName => displayName;
        public WorldRegionId Region => region;

        public void TryDiscover(Vector3 playerPosition)
        {
            if (IsDiscovered || Vector3.Distance(transform.position, playerPosition) > discoveryRadius) return;
            IsDiscovered = true;
            Discovered?.Invoke(this);
        }

        public void RestoreDiscovered() => IsDiscovered = true;
    }
}
