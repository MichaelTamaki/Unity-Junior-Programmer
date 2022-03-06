using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
class SaveData
{
    public string name;
    public int score;
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }
    private SaveData highScoreData;
    public string activeName;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
    }

    private string GetSaveFilePath()
    {
        return Application.persistentDataPath + "savedata.json";
    }

    public void SetName(string name)
    {
        activeName = name;
    }

    public void SaveHighScore(int score)
    {
        SaveData saveData = new SaveData();
        saveData.name = activeName;
        saveData.score = score;
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(GetSaveFilePath(), json);
        highScoreData = saveData;
    }

    public void LoadHighScore()
    {
        if (File.Exists(GetSaveFilePath()))
        {
            string json = File.ReadAllText(GetSaveFilePath());
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            SetName(saveData.name);
            highScoreData = saveData;
        }
    }

    public string GetCurrentHighScoreName()
    {
        if (highScoreData != null)
        {
            return highScoreData.name;
        } else
        {
            return "";
        }
    }

    public int GetCurrentHighScore()
    {
        if (highScoreData != null)
        {
            return highScoreData.score;
        }
        else
        {
            return 0;
        }
    }
}
