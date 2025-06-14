using System.Text;
using UnityEngine;

public enum ItemType
{
    Material,
    Equipment
}
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    [Range(0,100)]
    public int dropRate;

    protected StringBuilder sb = new StringBuilder();

    public virtual string GetDescription()
    {
        return "";
    }
    
}
