using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public ItemSO itemSO;

    public void Pickup()
    {
        InventoryUI.Instance.AddItem(itemSO);
        Destroy(gameObject);
    }
}
