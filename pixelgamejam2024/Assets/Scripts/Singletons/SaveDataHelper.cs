using System;
using Gley.AllPlatformsSave;
using Multiplayer;
using UnityEngine;
using SaveResult = Gley.AllPlatformsSave.SaveResult;

public class PlayerData
{
    public GameConstants.CharacterTypes LastCharacterUsed = GameConstants.CharacterTypes.Alpha;
}

[Serializable]
public class DeviceData
{
    // Audio
    public float GlobalVolume;
    public float MusicVolume;
    public float SfxVolume;
}

public static class SaveDataHelper
{
    private static readonly string StoredSaveDataPath = Application.persistentDataPath + "/" + "SaveData";

    public static Action OnDataLoadComplete;
    public static Action OnDataSaveComplete;

    public static readonly PlayerData ActivePlayerData = new();
    public static DeviceData ActiveDeviceData { get; private set; }
    public static bool InProgress { get; private set; }

    public static void LoadSaveData()
    {
        GetSaveDataLocal();
    }

    public static void SaveDeviceData()
    {
        if (!InProgress)
        {
            InProgress = true;
            API.Save(ActiveDeviceData, StoredSaveDataPath, OnDataSaved, true);
        }
    }

    private static void GetSaveDataLocal()
    {
        if (!InProgress)
        {
            InProgress = true;
            API.Load<DeviceData>(StoredSaveDataPath, OnDataLoaded, true);
        }
    }

    private static void OnDataLoaded(DeviceData loaded, SaveResult result, string message)
    {
        ActiveDeviceData = result switch
        {
            SaveResult.EmptyData or SaveResult.Error => new DeviceData(),
            SaveResult.Success => loaded,
            _ => ActiveDeviceData
        };
        InProgress = false;
        OnDataLoadComplete?.Invoke();
    }

    private static void OnDataSaved(SaveResult result, string message)
    {
        if (result == SaveResult.Error)
            // log error 
            Debug.Log(message);
        InProgress = false;
        OnDataSaveComplete?.Invoke();
    }
}