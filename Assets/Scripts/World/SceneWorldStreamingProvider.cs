using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurfaceRiseOfHumanity.World
{
    public interface IWorldStreamingProvider
    {
        IEnumerator LoadChunk(WorldChunk chunk);
        IEnumerator UnloadChunk(WorldChunk chunk);
        bool IsLoaded(string chunkId);
        IReadOnlyCollection<string> GetLoadedChunks();
    }

    public sealed class SceneWorldStreamingProvider : MonoBehaviour, IWorldStreamingProvider
    {
        private readonly HashSet<string> loaded = new HashSet<string>();
        private readonly Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        public IReadOnlyCollection<string> GetLoadedChunks() => loaded;
        public bool IsLoaded(string chunkId) => loaded.Contains(chunkId);

        public IEnumerator LoadChunk(WorldChunk chunk)
        {
            if (chunk == null || string.IsNullOrWhiteSpace(chunk.chunkId) || loaded.Contains(chunk.chunkId)) yield break;
            if (string.IsNullOrWhiteSpace(chunk.sceneName)) { loaded.Add(chunk.chunkId); yield break; }
            AsyncOperation operation = SceneManager.LoadSceneAsync(chunk.sceneName, LoadSceneMode.Additive);
            if (operation == null) yield break;
            while (!operation.isDone) yield return null;
            Scene scene = SceneManager.GetSceneByName(chunk.sceneName);
            scenes[chunk.chunkId] = scene;
            loaded.Add(chunk.chunkId);
        }

        public IEnumerator UnloadChunk(WorldChunk chunk)
        {
            if (chunk == null || !loaded.Remove(chunk.chunkId)) yield break;
            if (scenes.TryGetValue(chunk.chunkId, out Scene scene) && scene.IsValid() && scene.isLoaded)
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                if (operation != null) while (!operation.isDone) yield return null;
            }
            scenes.Remove(chunk.chunkId);
        }
    }
}
