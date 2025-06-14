using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
        
    }



    public void UpdateSlot(InventoryItem _item)
    {


        item = _item;
        itemImage.color = Color.white;
        if(item != null)
        {
            itemImage.sprite = item.data.icon;
            if(item.stackSize > 1)
                itemText.text = item.stackSize.ToString();
            else
                itemText.text = "";
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(item == null) return;
        if(Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }
        if(item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
        }
        ui.itemToolTip.HideToolTip();
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if(item == null) return;
        Vector2 mousePosition = Input.mousePosition;

        float xOffset = 0;
        float yOffset = 0;
        if(mousePosition.x>600)
            xOffset = -75;
        else
            xOffset = 75;    

        if(mousePosition.y>320)
            yOffset = -150;
        else
            yOffset = 150;

        ui.itemToolTip.ShowIoolTip(item.data);
        ui.itemToolTip.transform.position = new Vector2(mousePosition.x+xOffset,mousePosition.y+yOffset);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(item == null) return;
        ui.itemToolTip.HideToolTip();
    }

}
