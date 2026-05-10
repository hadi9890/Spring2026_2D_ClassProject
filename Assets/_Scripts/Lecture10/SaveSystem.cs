using System;
using UnityEngine;
using System.IO;

namespace _Scripts.Lecture10
{
    public class SaveSystem : MonoBehaviour
    {
        private static SavedData savedData = new();

        [Serializable]
        public struct SavedData
        {
            public PlayerSave.PlayerData playerSaveData;
        }

        public static string SaveFileName()
        {
            var saveFile = Application.persistentDataPath + "/save" + ".sav";
            // Debug.Log(saveFile);
            return saveFile;
        }

        private static void HandleSaveData()
        {
            PlayerSave.Instance.Save(ref savedData.playerSaveData);
        }

        private static void HandleLoadData()
        {
            PlayerSave.Instance.Load(savedData.playerSaveData);
        }

        public static void Save()
        {
            HandleSaveData();

            File.WriteAllText(SaveFileName(), JsonUtility.ToJson(savedData, false));
            // Debug.Log("Saving data...");
        }

        public static void Load()
        {
            var saveContent = File.ReadAllText(SaveFileName());
            savedData = JsonUtility.FromJson<SavedData>(saveContent);

            HandleLoadData();

            // Debug.Log("Loading data...");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                Load();
            }
        }
    }
}