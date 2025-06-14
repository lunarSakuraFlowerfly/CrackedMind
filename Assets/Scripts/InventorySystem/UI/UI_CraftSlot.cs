using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UI_CraftSlot : UI_ItemSlot
{
    private void OnEnable()
    {
        UpdateSlot(item);
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if(_data == null) return;
        item.data = _data;
        itemImage.sprite = _data.icon;
        itemText.text = _data.itemName;

        if(itemText.text.Length > 11)
            itemText.fontSize = itemText.fontSize * 0.7f;
        else
            itemText.fontSize = 28;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}