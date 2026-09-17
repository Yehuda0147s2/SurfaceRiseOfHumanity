using UnityEngine;
using SurfaceRiseOfHumanity.Core;

namespace SurfaceRiseOfHumanity.Encounters
{
    public enum EncounterType { Patrol, Guard, Investigation, Ambush }
    public enum RespawnPolicy { Never, OnReload, Timed }

    [CreateAssetMenu(menuName = "Surface/Encounter Definition", fileName = "EncounterDefinition")]
    public sealed class EncounterDefinition : ScriptableObject
    {
        public string encounterId;
        public WorldRegionId region = WorldRegionId.GreenZone;
        public string chunkId;
        public EncounterType encounterType;
        public Transform spawnPoint;
        public Transform[] patrolRoute;
        [Min(1)] public int enemyCount = 1;
        public float activationDistance = 75f;
        public RespawnPolicy respawnPolicy = RespawnPolicy.OnReload;
        public string associatedPoiId;
        public string rewardItemId;
        public string requiredMissionId;
    }

    public sealed class EncounterZone : MonoBehaviour
    {
        [SerializeField] private EncounterDefinition definition;
        [SerializeField] private GameObject robotScoutPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform player;
        private bool activated;
        public string EncounterId => definition == null ? name : definition.encounterId;

        private void Update()
        {
            if (activated || definition == null || player == null) return;
            if (Vector3.Distance(transform.position, player.position) > definition.activationDistance) return;
            Activate();
        }

        public void Activate()
        {
            if (activated || robotScoutPrefab == null) return;
            activated = true;
            int count = Mathf.Min(definition.enemyCount, spawnPoints == null ? 0 : spawnPoints.Length);
            for (int i = 0; i < count; i++) Instantiate(robotScoutPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
        }
    }
}
