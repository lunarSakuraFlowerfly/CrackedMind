using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailUI : MonoBehaviour
{
    public static ItemDetailUI Instance { get; private set; }
    private ItemSO itemSO;
    private ItemUI itemUI;
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI descriptionText;
    public Transform propertyList;
    public GameObject propertyTemplate;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    #region 更新UI
    public void UpdateDetailUI(ItemSO itemSO,ItemUI itemUI)
    {
        if (this.itemUI == itemUI)
        {
            gameObject.SetActive(!gameObject.activeSelf);
            return;
        }
        else
        {
            gameObject.SetActive(true);
            this.itemUI = itemUI;
            this.itemSO = itemSO;
        }

        iconImage.sprite = itemSO.icon;
        nameText.text = itemSO.Name;
        switch (itemSO.itemType)
        {
            case ItemType.Weapon:
                itemType.text = "武器";
                break;
            case ItemType.Consumeable:
                itemType.text = "消耗品";
                break;
        }
        descriptionText.text = itemSO.description;
        foreach (var item in itemSO.propertyList)
        {
            GameObject property = Instantiate(propertyTemplate, propertyList);
            string itemType = "";
            switch (item.propertyType)
            {
                case PropertyType.HpValue:
                    itemType = "生命值: ";
                    break;
                //case PropertyType.MentalValue:
                //    itemType = "精神值: ";
                //    break;
                case PropertyType.MagicValue:
                    itemType = "法力值: ";
                    break;
                case PropertyType.MoveSpeed:
                    itemType = "速度值: ";
                    break;
                case PropertyType.HungryValue:
                    itemType = "饱食度: ";
                    break;
                case PropertyType.AttackValue:
                    itemType = "攻击力: ";
                    break;
                case PropertyType.DefensiveValue:
                    itemType = "防御力: ";
                    break;
            }
            property.GetComponent<TextMeshProUGUI>().text = itemType + item.value;
            property.SetActive(true);
        }
    }
    #endregion

    #region 按钮

    public void OnUseButtonClick()
    {
        InventoryUI.Instance.UseItem(itemSO,itemUI);
    }
    public void OnThrowButtonClick()
    {
        InventoryUI.Instance.ThrowItem(itemSO,itemUI);
    }
    public void OnSellButtonClick()
    {

    }
    #endregion
}
