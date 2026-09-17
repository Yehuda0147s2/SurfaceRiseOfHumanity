using System;
using System.Collections.Generic;
using UnityEngine;
using SurfaceRiseOfHumanity.Core;
using SurfaceRiseOfHumanity.Inventory;

namespace SurfaceRiseOfHumanity.WorldState
{
    [Serializable] public sealed class SaveMetadata { public int saveFormatVersion = 1; public string saveId; public string createdUtc; public string lastSavedUtc; public string currentScene; public WorldRegionId currentRegion; public string applicationVersion; }
    [Serializable] public sealed class PlayerState { public Vector3 position; public Vector3 rotationEuler; public string currentChunk; public int level = 1; public int experience; public int skillPoints; public int health = 100; public int maxHealth = 100; public int armor; public float energy = 100f; public float maxEnergy = 100f; public float stamina = 100f; public float maxStamina = 100f; public int combatSkill; public int survivalSkill; public int technologySkill; public string currentWeapon = "AR7_SURVIVOR_RIFLE"; public int magazine = 30; public int reserveAmmo = 120; }
    [Serializable] public sealed class MissionState { public string activeMission = "M01_THE_SEALED_PATH"; public List<string> completed = new List<string>(); public List<string> objectiveFlags = new List<string>(); public List<string> storyFlags = new List<string>(); }
    [Serializable] public sealed class IdRecord { public string id; public string firstSeenUtc; public IdRecord() { } public IdRecord(string value) { id = value; firstSeenUtc = DateTime.UtcNow.ToString("O"); } }
    [Serializable] public sealed class DiscoveryState { public List<IdRecord> discovered = new List<IdRecord>(); public bool Has(string id) => discovered != null && discovered.Exists(x => x != null && x.id == id); public void Add(string id) { if (!string.IsNullOrWhiteSpace(id) && !Has(id)) { if (discovered == null) discovered = new List<IdRecord>(); discovered.Add(new IdRecord(id)); } } }
    [Serializable] public sealed class ScannerState { public List<string> scannedObjectIds = new List<string>(); public int rawMachineData; public bool scannerRangeUpgrade; public bool scanSpeedUpgrade; public void AddScan(string id) { if (!string.IsNullOrWhiteSpace(id) && (scannedObjectIds == null || !scannedObjectIds.Contains(id))) { if (scannedObjectIds == null) scannedObjectIds = new List<string>(); scannedObjectIds.Add(id); } } }
    [Serializable] public sealed class MachineKnowledgeState { public List<MachineKnowledgeRecord> machines = new List<MachineKnowledgeRecord>(); public MachineKnowledgeRecord GetOrCreate(string id) { if (machines == null) machines = new List<MachineKnowledgeRecord>(); MachineKnowledgeRecord value = machines.Find(x => x != null && x.machineId == id); if (value == null) { value = new MachineKnowledgeRecord { machineId = id }; machines.Add(value); } return value; } }
    [Serializable] public sealed class MachineKnowledgeRecord { public string machineId; public bool encountered; public bool scanned; public bool analysisComplete; public bool behaviorKnown; public bool attackPatternKnown; public bool weakPointKnown; public bool technologyKnown; }
    [Serializable] public sealed class InventoryState { public List<InventoryRecord> items = new List<InventoryRecord>(); }
    [Serializable] public sealed class InventoryRecord { public string itemId; public InventoryCategory category; public int quantity; }
    [Serializable] public sealed class EquipmentState { public string weapon = "AR7_SURVIVOR_RIFLE"; public string helmet; public string chest; public string gloves; public string legs; public string boots; public string backpack; public List<string> weaponUpgrades = new List<string>(); public List<string> armorUpgrades = new List<string>(); }
    [Serializable] public sealed class EncounterState { public List<EncounterRecord> encounters = new List<EncounterRecord>(); }
    [Serializable] public sealed class EncounterRecord { public string encounterId; public string status; public bool completed; }
    [Serializable] public sealed class WorldEventState { public List<string> completed = new List<string>(); public List<string> active = new List<string>(); }
    [Serializable] public sealed class RegionState { public WorldRegionId regionId; public bool unlocked; public bool discovered; }
    [Serializable] public sealed class ChunkState { public string chunkId; public bool discovered; public List<string> persistentObjectIds = new List<string>(); }
    [Serializable] public sealed class WorldState { public List<RegionState> regions = new List<RegionState>(); public List<ChunkState> chunks = new List<ChunkState>(); public List<string> activatedTerminals = new List<string>(); public List<string> openedGates = new List<string>(); public List<string> defeatedImportantEnemies = new List<string>(); public bool firstLightSeen; public bool unknownMachineSeen; }
    [Serializable] public sealed class WorldTimeState { public float normalizedTime = 0.25f; public int day; public string weather = "Clear"; }

    [Serializable] public sealed class GameWorldState
    {
        public SaveMetadata metadata = new SaveMetadata(); public PlayerState player = new PlayerState(); public MissionState missions = new MissionState(); public WorldState world = new WorldState(); public DiscoveryState discoveries = new DiscoveryState(); public ScannerState scanner = new ScannerState(); public MachineKnowledgeState machineKnowledge = new MachineKnowledgeState(); public InventoryState inventory = new InventoryState(); public EquipmentState equipment = new EquipmentState(); public EncounterState encounters = new EncounterState(); public WorldEventState events = new WorldEventState(); public WorldTimeState time = new WorldTimeState();
        public static GameWorldState CreateNew()
        {
            GameWorldState state = new GameWorldState(); state.metadata.saveId = Guid.NewGuid().ToString("N"); state.metadata.createdUtc = DateTime.UtcNow.ToString("O");
            state.world.regions.Add(new RegionState { regionId = WorldRegionId.TheDeep, unlocked = true, discovered = true }); state.world.regions.Add(new RegionState { regionId = WorldRegionId.TheAscent }); state.world.regions.Add(new RegionState { regionId = WorldRegionId.GreenZone });
            state.inventory.items.Add(new InventoryRecord { itemId = "AR7_SURVIVOR_RIFLE", category = InventoryCategory.Weapons, quantity = 1 }); state.inventory.items.Add(new InventoryRecord { itemId = "BASIC_SURVIVAL_SUIT", category = InventoryCategory.Armor, quantity = 1 }); return state;
        }
    }
}
