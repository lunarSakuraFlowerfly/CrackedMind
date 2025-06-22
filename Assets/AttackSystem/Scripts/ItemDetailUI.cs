using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InteractionSystem.Data;

public class ItemDetailUI : MonoBehaviour
{
    public static ItemDetailUI Instance { get; private set; }
    private ItemData itemData;
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
    public void UpdateDetailUI(ItemData itemData, ItemUI itemUI)
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
            this.itemData = itemData;
        }

        iconImage.sprite = itemData.icon;
        nameText.text = itemData.itemName;
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Equipment:
                itemType.text = "武器";
                break;
            case ItemData.ItemType.Consumable:
                itemType.text = "消耗品";
                break;
            default:
                itemType.text = itemData.itemType.ToString();
                break;
        }
        descriptionText.text = itemData.description;
        
        // 清除旧的属性列表
        foreach (Transform child in propertyList)
        {
            if (child.gameObject != propertyTemplate)
                Destroy(child.gameObject);
        }
        
        // 添加新的属性
        foreach (var item in itemData.propertyList)
        {
            GameObject property = Instantiate(propertyTemplate, propertyList);
            string itemTypeName = "";
            switch (item.propertyType)
            {
                case PropertyType.HpValue:
                    itemTypeName = "生命值: ";
                    break;
                case PropertyType.MentalValue:
                    itemTypeName = "精神值: ";
                    break;
                case PropertyType.MoveSpeed:
                    itemTypeName = "速度值: ";
                    break;
                case PropertyType.AttackValue:
                    itemTypeName = "攻击力: ";
                    break;
                case PropertyType.DefensiveValue:
                    itemTypeName = "防御力: ";
                    break;
                default:
                    itemTypeName = item.propertyType.ToString() + ": ";
                    break;
            }
            property.GetComponent<TextMeshProUGUI>().text = itemTypeName + item.value;
            property.SetActive(true);
        }
    }
    #endregion

    #region 按钮

    public void OnUseButtonClick()
    {
        InventoryUI.Instance.UseItem(itemData, itemUI);
    }
    public void OnThrowButtonClick()
    {
        InventoryUI.Instance.ThrowItem(itemData, itemUI);
    }
    public void OnSellButtonClick()
    {
        // 卖出物品逻辑
    }
    #endregion
}
