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

    #region ����UI
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
                itemType.text = "����";
                break;
            case ItemType.Consumeable:
                itemType.text = "����Ʒ";
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
                    itemType = "����ֵ: ";
                    break;
                //case PropertyType.MentalValue:
                //    itemType = "����ֵ: ";
                //    break;
                case PropertyType.MagicValue:
                    itemType = "����ֵ: ";
                    break;
                case PropertyType.MoveSpeed:
                    itemType = "�ٶ�ֵ: ";
                    break;
                case PropertyType.HungryValue:
                    itemType = "��ʳ��: ";
                    break;
                case PropertyType.AttackValue:
                    itemType = "������: ";
                    break;
                case PropertyType.DefensiveValue:
                    itemType = "������: ";
                    break;
            }
            property.GetComponent<TextMeshProUGUI>().text = itemType + item.value;
            property.SetActive(true);
        }
    }
    #endregion

    #region ��ť

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
