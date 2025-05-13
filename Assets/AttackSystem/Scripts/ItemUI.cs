using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InteractionSystem.Data;

public class ItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public ItemData itemData;

    public void InitItem(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Equipment:
                typeText.text = "武器";
                break;
            case ItemData.ItemType.Consumable:
                typeText.text = "消耗品";
                break;
            default:
                typeText.text = itemData.itemType.ToString();
                break;
        }
        iconImage.sprite = itemData.icon;
        nameText.text = itemData.itemName;
        this.itemData = itemData;
    }
    
    public void OnClickItem()
    {
        InventoryUI.Instance.ShowItemDetailUI(itemData, this);
    }
}
