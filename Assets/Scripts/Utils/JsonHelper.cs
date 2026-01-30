using UnityEngine;
using System.IO;
using System;

public class JsonHelper
{
    private static string SaveDirectory => Path.Combine(Application.persistentDataPath, "saves");
    public static void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }
    public static string GetSavePath(string fileName)
    {
        EnsureSaveDirectoryExists();
        return Path.Combine(SaveDirectory, $"{fileName}.json");
    }
    public static void SaveToJson<T>(T data, string fileName)
    {
        try
        {
            string filePath = GetSavePath(fileName);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);

            Debug.Log($"数据已保存到: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
        }
    }
    public static T LoadFromJson<T>(string fileName) where T : new()
    {
        try
        {
            string filePath = GetSavePath(fileName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"文件不存在: {filePath}");
                return new T();
            }

            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return new T();
        }
    }
}