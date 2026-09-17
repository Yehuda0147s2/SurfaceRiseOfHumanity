using System;
using UnityEngine;
using SurfaceRiseOfHumanity.Discovery;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.Scanner
{
    /// <summary>Runtime adapter that writes scanner results to the authoritative GameWorldState.</summary>
    public sealed class MachineDataLedger : MonoBehaviour
    {
        [SerializeField] private ScannerController scanner;
        [SerializeField] private WorldStateManager worldState;
        public event Action<string, bool> KnowledgeChanged;

        private void Awake()
        {
            if (scanner == null) scanner = GetComponent<ScannerController>();
            if (worldState == null) worldState = FindObjectOfType<WorldStateManager>();
        }

        private void OnEnable() { if (scanner != null) scanner.ScanResult += RecordScan; }
        private void OnDisable() { if (scanner != null) scanner.ScanResult -= RecordScan; }

        private void RecordScan(Scannable target, ScanKnowledge knowledge)
        {
            if (target == null || worldState == null || worldState.State == null) return;
            MachineKnowledgeRecord record = worldState.State.machineKnowledge.GetOrCreate(target.DiscoveryId);
            bool wasNew = !record.scanned || (knowledge == ScanKnowledge.AnalysisComplete && !record.analysisComplete);
            record.encountered = true;
            record.scanned = true;
            record.analysisComplete |= knowledge == ScanKnowledge.AnalysisComplete;
            worldState.State.scanner.AddScan(target.DiscoveryId);
            if (wasNew) worldState.AddMachineData(1);
            KnowledgeChanged?.Invoke(target.DiscoveryId, wasNew);
        }
    }
}
