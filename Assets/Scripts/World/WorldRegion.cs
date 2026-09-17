using UnityEngine;
using SurfaceRiseOfHumanity.Core;

namespace SurfaceRiseOfHumanity.World
{
    [CreateAssetMenu(menuName = "Surface/World Region", fileName = "WorldRegion")]
    public sealed class WorldRegion : ScriptableObject
    {
        public WorldRegionId regionId;
        public string displayName;
        [Min(0)] public float loadDistance = 250f;
        [Min(0)] public float unloadDistance = 350f;
        public WorldChunk[] chunks;
    }
}
