using UnityEngine;

public interface ISaveManager
{
    void SaveData(ref GameData data);
    void LoadData(GameData data);
}