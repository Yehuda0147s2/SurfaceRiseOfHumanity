using System;
using System.IO;
using UnityEngine;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.Save
{
    public sealed class VersionedSaveService
    {
        public const int CurrentVersion = 1;
        private readonly string savePath;
        private readonly string backupPath;
        private readonly string tempPath;

        public VersionedSaveService(string profile = "default")
        {
            string root = Application.persistentDataPath;
            savePath = Path.Combine(root, "surface_" + profile + ".save.json");
            backupPath = savePath + ".backup";
            tempPath = savePath + ".tmp";
        }

        public bool Save(GameWorldState state)
        {
            if (!SaveValidator.IsValid(state)) return false;
            state.metadata.saveFormatVersion = CurrentVersion;
            state.metadata.lastSavedUtc = DateTime.UtcNow.ToString("O");
            string json = JsonUtility.ToJson(state, true);
            try
            {
                File.WriteAllText(tempPath, json);
                GameWorldState check = JsonUtility.FromJson<GameWorldState>(File.ReadAllText(tempPath));
                if (!SaveValidator.IsValid(check)) return false;
                if (File.Exists(savePath)) File.Copy(savePath, backupPath, true);
                File.Copy(tempPath, savePath, true);
                File.Delete(tempPath);
                return true;
            }
            catch (Exception error) { Debug.LogError($"Save failed: {error.Message}"); return false; }
        }

        public bool TryLoad(out GameWorldState state)
        {
            state = null;
            string source = File.Exists(savePath) ? savePath : backupPath;
            if (!File.Exists(source)) return false;
            try
            {
                state = JsonUtility.FromJson<GameWorldState>(File.ReadAllText(source));
                state = SaveMigrationManager.Migrate(state);
                if (!SaveValidator.IsValid(state)) { state = null; return false; }
                return true;
            }
            catch (Exception error) { Debug.LogError($"Load failed safely: {error.Message}"); state = null; return false; }
        }

        public bool HasSave() => File.Exists(savePath) || File.Exists(backupPath);
        public bool DeleteSave() { try { if (File.Exists(savePath)) File.Delete(savePath); if (File.Exists(backupPath)) File.Delete(backupPath); return true; } catch { return false; } }
    }
}
