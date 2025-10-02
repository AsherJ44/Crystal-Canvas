using System.Collections;
using UnityEngine;
using System.IO;
using UnityEditor.Overlays;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath => Path.Combine(Application.persistentDataPath, "savefile.json");
    public static bool hasLoadedThisSession = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private IEnumerator Start()
    {
        yield return null;

        if (!hasLoadedThisSession) 
        {
            LoadGame();
            hasLoadedThisSession = true;
        }
    }

    public void SaveGame()
    {
        Debug.Log("Game Saved to " + savePath);

        if (GlobalInstanceManager.Instance == null)
        {
            Debug.LogWarning("GIM is null, cannot save");
            return;
        }

        SaveData data = new SaveData();
    
        var global = GlobalInstanceManager.Instance;
        data.CameraCaptureIndex = global.CameraCaptureIndex;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No Save File Found");
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            var global = GlobalInstanceManager.Instance;
            global.CameraCaptureIndex = data.CameraCaptureIndex;

            Debug.Log("Game Loaded");

        }
        catch (Exception)
        {
            Debug.LogError("Save Failed to " + savePath);
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save deleted.");
        }
    }
}