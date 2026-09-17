using System;
using UnityEngine;

namespace SurfaceRiseOfHumanity.NPC
{
    public sealed class NPCInteractable : MonoBehaviour
    {
        [SerializeField] private string npcName = "Settlement Resident";
        [SerializeField, TextArea] private string dialogue = "The surface is only a story. Keep to the safe levels.";
        [SerializeField] private string faction = "Settlement";
        public event Action<string, string> DialogueRequested;
        public string NpcName => npcName;
        public string Faction => faction;
        public void Interact() => DialogueRequested?.Invoke(npcName, dialogue);
    }
}
