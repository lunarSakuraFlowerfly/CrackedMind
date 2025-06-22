using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/Item Effect")]
public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("执行物品效果");
    }
}