using System.Collections.Generic;
using UnityEngine;
using SurfaceRiseOfHumanity.Core;
using SurfaceRiseOfHumanity.Exploration;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.Map
{
    public sealed class MapDataProvider : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private WorldStateManager worldState;
        [SerializeField] private PointOfInterest[] pointsOfInterest;
        public Vector3 CurrentPlayerPosition => player == null ? Vector3.zero : player.position;
        public WorldRegionId CurrentRegion => worldState == null || worldState.State == null ? WorldRegionId.TheDeep : worldState.State.metadata.currentRegion;
        public IReadOnlyList<RegionState> Regions => worldState.State.world.regions;
        public bool IsPOIDiscovered(string poiId) => worldState != null && worldState.State != null && worldState.State.discoveries.Has(poiId);
        public IEnumerable<PointOfInterest> GetDiscoveredPOIs()
        {
            foreach (PointOfInterest poi in pointsOfInterest)
                if (poi != null && IsPOIDiscovered(poi.PoiId)) yield return poi;
        }
    }
}
