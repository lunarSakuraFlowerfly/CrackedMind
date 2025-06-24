using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete Save file")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath,fileName,encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if(instance!=null)
            Destroy(gameObject);
        else
            instance = this;

        dataHandler = new FileDataHandler(Application.persistentDataPath,fileName,encryptData);
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
        Debug.Log("新游戏数据已创建并保存到文件");
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if(this.gameData == null)
        {
            Debug.Log("No save data found");
            NewGame();
        }
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
        Debug.Log("Loaded currency: " + gameData.currency);
    }

    public void SaveGame()
    {
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
        Debug.Log("saved currency: " + gameData.currency);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveData(GameData data)
    {
        gameData = data;
    }


    private List<ISaveManager> FindAllSaveManagers()
    {
       IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
       return new List<ISaveManager>(saveManagers);
    }

    public bool HasSaveData()
    {
        if(dataHandler.Load()!=null)
        {
            return true;
        }
        return false;
    }
}
