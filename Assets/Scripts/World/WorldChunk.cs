using UnityEngine;

namespace SurfaceRiseOfHumanity.World
{
    [CreateAssetMenu(menuName = "Surface/World Chunk", fileName = "WorldChunk")]
    public sealed class WorldChunk : ScriptableObject
    {
        public string chunkId;
        public string sceneName;
        public Vector3 worldAnchor;
        public bool persistent;
    }
}
