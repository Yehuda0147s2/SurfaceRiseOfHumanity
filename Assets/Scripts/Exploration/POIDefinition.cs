using UnityEngine;
using SurfaceRiseOfHumanity.Core;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.Exploration
{
    [CreateAssetMenu(menuName = "Surface/POI Definition", fileName = "POIDefinition")]
    public sealed class POIDefinition : ScriptableObject
    {
        public string poiId;
        public string displayName;
        public WorldRegionId region = WorldRegionId.GreenZone;
        public string chunkId;
        [TextArea] public string description;
        public string mapIconId;
        public string storyClue;
        public string[] rewardItemIds;
        public string[] scanTargetIds;
        public string[] encounterIds;
        public float discoveryRadius = 18f;
    }

    public sealed class POIRegistry : MonoBehaviour
    {
        [SerializeField] private WorldStateManager worldState;
        [SerializeField] private PointOfInterest[] points;
        public PointOfInterest[] Points => points;

        private void Awake()
        {
            if (worldState == null) worldState = FindObjectOfType<WorldStateManager>();
        }

        public bool IsDiscovered(string id) => worldState != null && worldState.State != null && worldState.State.discoveries.Has(id);
        public void RestoreLoadedPOIs()
        {
            if (worldState == null || worldState.State == null) return;
            foreach (PointOfInterest point in points)
                if (point != null && IsDiscovered(point.PoiId)) point.RestoreDiscovered();
        }
    }
}
