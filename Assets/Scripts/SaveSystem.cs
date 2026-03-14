using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Simple JSON save/load helper.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private const string SaveFileName = "mutant_evolution_idle_save.json";

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        [Serializable]
        public class SaveData
        {
            public float biomass;
            public int dnaPoints;
            public List<string> purchasedMutationIds = new();
        }

        public SaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                return new SaveData();
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to load save file. {ex.Message}");
                return new SaveData();
            }
        }

        public void Save(SaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to save file. {ex.Message}");
            }
        }
    }
}
