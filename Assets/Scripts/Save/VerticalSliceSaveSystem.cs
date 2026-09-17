using System.Collections.Generic;
using UnityEngine;
using SurfaceRiseOfHumanity.Discovery;

namespace SurfaceRiseOfHumanity.Save
{
    public sealed class VerticalSliceSaveSystem : MonoBehaviour
    {
        private const string SaveKey = "surface_vertical_slice_save";
        [SerializeField] private SurfaceRiseOfHumanity.Progression.KaelProgression progression;
        private SaveData data = new SaveData();

        [System.Serializable]
        private sealed class SaveData
        {
            public int level;
            public int experience;
            public int skillPoints;
            public List<string> discoveries = new List<string>();
        }

        public void Save()
        {
            if (progression != null)
            {
                data.level = progression.Level;
                data.experience = progression.Experience;
                data.skillPoints = progression.SkillPoints;
            }
            data.discoveries.Clear();
            foreach (Scannable item in FindObjectsOfType<Scannable>())
                if (item.Knowledge != ScanKnowledge.Unknown) data.discoveries.Add(item.DiscoveryId);
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public bool Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return false;
            data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey));
            if (data == null) return false;
            foreach (Scannable item in FindObjectsOfType<Scannable>())
                if (data.discoveries.Contains(item.DiscoveryId)) item.Scan();
            return true;
        }
    }
}
