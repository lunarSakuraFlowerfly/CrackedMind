using System.Text;
using UnityEngine;
using UnityEditor;

public enum ItemType
{
    Material,
    Equipment
}
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemDataSO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemID;

    [Range(0,100)]
    public int dropRate;

    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
        #if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);
        #endif
    }

    public virtual string GetDescription()
    {
        return "";
    }
    
}
