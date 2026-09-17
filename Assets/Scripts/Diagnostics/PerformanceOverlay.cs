using UnityEngine;
using UnityEngine.Profiling;
using SurfaceRiseOfHumanity.World;

namespace SurfaceRiseOfHumanity.Diagnostics
{
    public sealed class PerformanceOverlay : MonoBehaviour
    {
        [SerializeField] private bool enabledInDevelopmentBuilds = true;
        [SerializeField] private SceneWorldStreamingProvider streamingProvider;
        private GUIStyle style;

        private void OnGUI()
        {
            if (!enabledInDevelopmentBuilds || !Debug.isDebugBuild) return;
            if (style == null) { style = new GUIStyle(GUI.skin.label); style.fontSize = 18; style.normal.textColor = Color.white; }
            int chunks = streamingProvider == null ? 0 : streamingProvider.GetLoadedChunks().Count;
            GUI.Label(new Rect(12f, 12f, 600f, 120f), $"FPS: {1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f):0}\nFrame: {Time.unscaledDeltaTime * 1000f:0.0} ms\nMemory: {Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f):0} MB\nLoaded chunks: {chunks}", style);
        }
    }
}
