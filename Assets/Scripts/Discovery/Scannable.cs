using System;
using UnityEngine;

namespace SurfaceRiseOfHumanity.Discovery
{
    public enum ScanKnowledge { Unknown, Scanned, AnalysisComplete }

    public sealed class Scannable : MonoBehaviour
    {
        [SerializeField] private string discoveryId;
        [SerializeField] private string unknownName = "UNKNOWN MACHINE";
        [SerializeField] private string identifiedName = "ROBOT SCOUT";
        [SerializeField] private float scanDuration = 2f;

        public string DiscoveryId => string.IsNullOrEmpty(discoveryId) ? gameObject.name : discoveryId;
        public ScanKnowledge Knowledge { get; private set; }
        public event Action<Scannable, ScanKnowledge> ScanCompleted;

        public bool Scan()
        {
            Knowledge = Knowledge == ScanKnowledge.Unknown ? ScanKnowledge.Scanned : ScanKnowledge.AnalysisComplete;
            ScanCompleted?.Invoke(this, Knowledge);
            return true;
        }

        public void RestoreKnowledgeInternal(ScanKnowledge knowledge)
        {
            Knowledge = knowledge;
            ScanCompleted?.Invoke(this, Knowledge);
        }

        public string GetDisplayName() => Knowledge == ScanKnowledge.Unknown ? unknownName : identifiedName;
        public float ScanDuration => scanDuration;
    }
}
