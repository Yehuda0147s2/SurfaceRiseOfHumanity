using UnityEngine;
using SurfaceRiseOfHumanity.Discovery;

namespace SurfaceRiseOfHumanity.Discovery
{
    public static class ScannableStateExtensions
    {
        public static void RestoreKnowledge(this Scannable target, ScanKnowledge knowledge)
        {
            if (target == null) return;
            target.RestoreKnowledgeInternal(knowledge);
        }
    }
}
