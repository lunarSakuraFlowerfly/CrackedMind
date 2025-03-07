using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public ItemSO itemSO;

    public void InitItem(ItemSO itemSO)
    {
        switch (itemSO.itemType)
        {
            case ItemType.Weapon:
                typeText.text = "ÎäÆ÷";
                break;
            case ItemType.Consumeable:
                typeText.text = "ÏûºÄÆ·";
                break;
        }
        iconImage.sprite = itemSO.icon;
        nameText.text = itemSO.Name;
        this.itemSO = itemSO;
    }
    public void OnClickItem()
    {
        InventoryUI.Instance.ShowItemDetailUI(itemSO,this);
    }
}
