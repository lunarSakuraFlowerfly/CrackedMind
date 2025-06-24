using UnityEngine;
using System.IO;
using System;

public class FileDataHandler
{
    public string dataDirPath = " ";
    private string dataFileName = " ";

    private bool encryptData = false;
    private string codeWord = "alexdev";


    public FileDataHandler(string _dataDirPath,string _dataFileName,bool _encryptData = false)
    {
        this.dataDirPath = _dataDirPath;
        this.dataFileName = _dataFileName;
        this.encryptData = _encryptData;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        Debug.Log("Saving data to " + fullPath);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(_data,true);
            if(encryptData)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using(FileStream stream = new FileStream(fullPath,FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch(Exception e)
        {
            Debug.LogError("Error saving data to " + fullPath + "\n" + e.Message);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        Debug.Log("Loading data from " + fullPath);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath,FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if(encryptData)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error loading data from " + fullPath + "\n" + e.Message);
            }
        }
        return loadedData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        if(File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";
        //j^r=1
        //j^1=r
        for(int i = 0;i<_data.Length;i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;


    }
}