using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    public void ShowIoolTip(ItemDataSO _itemData)
    {
        if(_itemData == null) return;
        gameObject.SetActive(true);

        itemNameText.text = _itemData.itemName;
        if(itemNameText.text.Length > 9)
            itemNameText.fontSize = itemNameText.fontSize * .7f;
        else
            itemNameText.fontSize = 36;    
        if(_itemData.itemType == ItemType.Equipment)
        {
            ItemData_Equipment _equipment = _itemData as ItemData_Equipment;
            itemTypeText.text = _equipment.equipmentType.ToString();
        }
        else
        {
            itemTypeText.text = _itemData.itemType.ToString();
        }
        itemDescriptionText.text = _itemData.GetDescription();
        AdjustPosition();
    }
    public void HideToolTip()=>gameObject.SetActive(false);
    
}
