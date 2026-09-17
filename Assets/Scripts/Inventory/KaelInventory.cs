using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceRiseOfHumanity.Inventory
{
    public enum InventoryCategory { Weapons, Armor, Resources, MachineComponents, MachineData, QuestItems }

    [Serializable]
    public sealed class InventoryEntry
    {
        public string itemId;
        public InventoryCategory category;
        public int quantity;
    }

    public sealed class KaelInventory : MonoBehaviour
    {
        [SerializeField] private int capacity = 24;
        [SerializeField] private List<InventoryEntry> entries = new List<InventoryEntry>();
        public IReadOnlyList<InventoryEntry> Entries => entries;
        public event Action<InventoryEntry> Changed;

        public bool Add(string itemId, InventoryCategory category, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0) return false;
            InventoryEntry entry = entries.Find(x => x.itemId == itemId && x.category == category);
            if (entry == null)
            {
                if (entries.Count >= capacity) return false;
                entry = new InventoryEntry { itemId = itemId, category = category, quantity = 0 };
                entries.Add(entry);
            }
            entry.quantity += quantity;
            Changed?.Invoke(entry);
            return true;
        }

        public bool Remove(string itemId, int quantity = 1)
        {
            InventoryEntry entry = entries.Find(x => x.itemId == itemId);
            if (entry == null || quantity <= 0 || entry.quantity < quantity) return false;
            entry.quantity -= quantity;
            if (entry.quantity == 0) entries.Remove(entry);
            Changed?.Invoke(entry);
            return true;
        }

        public int Count(string itemId)
        {
            InventoryEntry entry = entries.Find(x => x.itemId == itemId);
            return entry == null ? 0 : entry.quantity;
        }
    }
}
