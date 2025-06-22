using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currency;

    public SerializableDictionary<string,bool> skillTree;
    public SerializableDictionary<string,int> inventory;
    public List<string> equipmentId;
    public List<string> assignedSkills;
    public SerializableDictionary<string,bool> checkpoints;
    public string closestCheckpointId;

    public SerializableDictionary<string,float> volumeSettings;

    public GameData()
    {
        currency = 0;
        skillTree = new SerializableDictionary<string,bool>();
        inventory = new SerializableDictionary<string,int>();
        equipmentId = new List<string>();
        assignedSkills = new List<string>();
        checkpoints = new SerializableDictionary<string,bool>();
        closestCheckpointId = string.Empty;
        volumeSettings = new SerializableDictionary<string,float>();
    }
}
