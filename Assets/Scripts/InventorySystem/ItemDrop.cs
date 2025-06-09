using UnityEngine;
using System.Collections.Generic;
public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int dropAmount = 1;
    [SerializeField] private ItemDataSO[] possibleDrops;
    private List<ItemDataSO> drops = new List<ItemDataSO>();
    [SerializeField] private GameObject dropPrefab;
    
    private bool hasDropped = false;

    public virtual void GenerateDrops()
    {
        if(hasDropped) return;
        hasDropped = true;
        for(int i = 0; i < possibleDrops.Length; i++)
        {
            if(Random.Range(0,100) <= possibleDrops[i].dropRate)
                drops.Add(possibleDrops[i]);
        }
        for(int i = 0; i < dropAmount; i++)
        {
            ItemDataSO randomDrop = drops[Random.Range(0,drops.Count)];
            DropItem(randomDrop);
        }
    }

    protected void DropItem(ItemDataSO _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        
        Vector2 randomVelocity = new Vector2(Random.Range(-5,5),Random.Range(12,15));
        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
    
}