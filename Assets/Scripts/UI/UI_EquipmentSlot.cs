using UnityEngine;
using UnityEngine.EventSystems;
class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;
    private void OnValidate()
    {
        gameObject.name = "Equipment Slot - " + slotType.ToString();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(item==null||item.data==null) return;
        
        // 播放装备点击音效
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(4, null);
            
        //卸下装备
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        ui.itemToolTip.HideToolTip();
        CleanUpSlot();
    }
}