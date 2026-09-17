using System.Collections.Generic;
using UnityEngine;

namespace SurfaceRiseOfHumanity.World
{
    public sealed class WorldStreamingManager : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private WorldRegion[] regions;
        [SerializeField, Min(0.25f)] private float evaluationInterval = 1f;
        private readonly HashSet<string> loadedChunks = new HashSet<string>();
        private float evaluationTimer;

        private void Update()
        {
            if (player == null) return;
            evaluationTimer -= Time.deltaTime;
            if (evaluationTimer > 0f) return;
            evaluationTimer = evaluationInterval;
            EvaluateChunks();
        }

        private void EvaluateChunks()
        {
            foreach (WorldRegion region in regions)
            {
                if (region == null || region.chunks == null) continue;
                foreach (WorldChunk chunk in region.chunks)
                {
                    if (chunk == null || string.IsNullOrEmpty(chunk.chunkId)) continue;
                    float distance = Vector3.Distance(player.position, chunk.worldAnchor);
                    if (distance <= region.loadDistance) RequestLoad(chunk);
                    else if (distance >= region.unloadDistance && !chunk.persistent) RequestUnload(chunk);
                }
            }
        }

        private void RequestLoad(WorldChunk chunk)
        {
            if (loadedChunks.Add(chunk.chunkId))
                Debug.Log($"World chunk load requested: {chunk.chunkId} ({chunk.sceneName})", this);
        }

        private void RequestUnload(WorldChunk chunk)
        {
            if (loadedChunks.Remove(chunk.chunkId))
                Debug.Log($"World chunk unload requested: {chunk.chunkId}", this);
        }
    }
}
