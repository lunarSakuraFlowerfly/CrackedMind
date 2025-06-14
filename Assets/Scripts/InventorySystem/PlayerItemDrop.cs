using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseStashItems;


    List<InventoryItem> currentEquipment = new List<InventoryItem>();
    public override void GenerateDrops()
    {
        List<InventoryItem> currentEquipment = Inventory.instance.equipment;
        for(int i = 0; i < currentEquipment.Count; i++)
        {
            if(Random.Range(0,100) <= chanceToLooseItems)
            {
                DropItem(currentEquipment[i].data);
                Inventory.instance.RemoveFromEquipment(currentEquipment[i].data as ItemData_Equipment);
            }
        }
        List<InventoryItem> materalsToLoose = new List<InventoryItem>();
        
        foreach (InventoryItem item in Inventory.instance.stash)
        {
            if (Random.Range(0, 100) <= chanceToLooseStashItems)
            {
                DropItem(item.data);
                materalsToLoose.Add(item);
            }
        }
        foreach(InventoryItem item in materalsToLoose)
        {
            Inventory.instance.RemoveItem(item.data);
        }
    }
}
