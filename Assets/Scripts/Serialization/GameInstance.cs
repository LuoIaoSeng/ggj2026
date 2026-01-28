using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameInstance Instance { get; private set; }
    private static string dataFileName = "save";

    public GameData gameData
    {
        set;
        get;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadData();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SaveData()
    {
        // 添加时间戳
        gameData.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        gameData.saveVersion = Application.version;

        JsonHelper.SaveToJson<GameData>(gameData, dataFileName);
    }

    public void LoadData()
    {
        if (!File.Exists(JsonHelper.GetSavePath(dataFileName)))
        {
            gameData = new GameData();
            SaveData();
        }
        else
        {
            gameData = JsonHelper.LoadFromJson<GameData>(dataFileName);
        }
    }

}
