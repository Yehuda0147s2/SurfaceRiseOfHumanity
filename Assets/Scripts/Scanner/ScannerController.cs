using System;
using UnityEngine;
using SurfaceRiseOfHumanity.Discovery;
using SurfaceRiseOfHumanity.Progression;

namespace SurfaceRiseOfHumanity.Scanner
{
    public sealed class ScannerController : MonoBehaviour
    {
        [SerializeField] private Camera scanCamera;
        [SerializeField] private KaelOverclock overclock;
        [SerializeField] private float range = 35f;
        [SerializeField] private float scanDuration = 2f;
        [SerializeField] private LayerMask scanMask = ~0;
        private float progress;
        private Scannable target;
        private bool scanning;

        public event Action<string, float> ScanProgress;
        public event Action<Scannable, ScanKnowledge> ScanResult;
        public bool IsScanning => scanning;
        public string CurrentTargetName => target == null ? string.Empty : target.GetDisplayName();

        private void Awake()
        {
            if (scanCamera == null) scanCamera = Camera.main;
            if (overclock == null) overclock = GetComponent<KaelOverclock>();
        }

        private void Update()
        {
            if (scanCamera == null || scanning) return;
            target = FindTarget();
            if (Input.GetKeyDown(KeyCode.Q)) BeginScan();
        }

        public void BeginScan()
        {
            if (target == null || scanning) return;
            scanning = true;
            progress = 0f;
            StartCoroutine(ScanRoutine(target));
        }

        private System.Collections.IEnumerator ScanRoutine(Scannable scannedTarget)
        {
            float duration = scannedTarget.ScanDuration > 0f ? scannedTarget.ScanDuration : scanDuration;
            if (overclock != null) duration /= Mathf.Max(0.1f, overclock.ScannerMultiplier);
            while (progress < 1f && scannedTarget == target)
            {
                progress += Time.deltaTime / duration;
                ScanProgress?.Invoke(scannedTarget.GetDisplayName(), progress);
                yield return null;
            }
            if (scannedTarget == target)
            {
                scannedTarget.Scan();
                ScanResult?.Invoke(scannedTarget, scannedTarget.Knowledge);
            }
            scanning = false;
        }

        private Scannable FindTarget()
        {
            Ray ray = scanCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (!Physics.Raycast(ray, out RaycastHit hit, range, scanMask, QueryTriggerInteraction.Ignore)) return null;
            return hit.collider.GetComponentInParent<Scannable>();
        }
    }
}
