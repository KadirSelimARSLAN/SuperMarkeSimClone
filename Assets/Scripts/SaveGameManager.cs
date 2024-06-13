using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveGameManager
{
    public static GameData CurrentSaveData = new GameData();

    public const string SaveDirectory = "/SaveData/";
    public const string FileName = "SaveGame.sav";

    

    public static bool SaveGame()
    {
        var dir = Application.persistentDataPath + SaveDirectory;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(CurrentSaveData, true);
        File.WriteAllText(dir+ FileName,json);
        Debug.Log(dir);
        GUIUtility.systemCopyBuffer = dir;

        return true;
    }

    public static void LoadGame()
    {
        string fullPath = Application.persistentDataPath + SaveDirectory + FileName;
        GameData tempData = new GameData();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            tempData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Oyun yüklendi");
        }
        else
        {
            Debug.LogError("Save file does not exist!");
        }

        CurrentSaveData = tempData;

    }
   
}
