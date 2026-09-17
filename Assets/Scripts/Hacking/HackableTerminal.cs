using System;
using UnityEngine;

namespace SurfaceRiseOfHumanity.Hacking
{
    public enum HackResult { AccessRequired, AccessGranted, Disabled, Activated }

    public sealed class HackableTerminal : MonoBehaviour
    {
        [SerializeField] private string terminalId;
        [SerializeField] private bool requiresScan = true;
        public event Action<HackResult> HackingCompleted;
        public bool IsHacked { get; private set; }
        public string TerminalId => string.IsNullOrWhiteSpace(terminalId) ? gameObject.name : terminalId;

        public HackResult Hack(bool scanned)
        {
            if (IsHacked) return HackResult.AccessGranted;
            if (requiresScan && !scanned) return HackResult.AccessRequired;
            IsHacked = true;
            HackResult result = HackResult.Activated;
            HackingCompleted?.Invoke(result);
            return result;
        }
    }
}
