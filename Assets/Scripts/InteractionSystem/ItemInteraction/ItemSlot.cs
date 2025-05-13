using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InteractionSystem.Data;

namespace InteractionSystem.Item
{
    /// <summary>
    /// 物品槽组件，显示单个物品
    /// </summary>
    public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI引用")]
        // 移除图标引用
        // [SerializeField] private Image iconImage;
        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        
        [Header("悬停效果")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 1f);
        
        // 物品数据
        private ItemData itemData;
        
        // 物品信息UI引用
        private ItemInfoUI itemInfoUI;
        
        private void Start()
        {
            // 查找物品信息UI
            itemInfoUI = FindObjectOfType<ItemInfoUI>();
            
            // 默认隐藏名称文本
            if (nameText != null)
            {
                nameText.enabled = false;
            }
        }
        
        /// <summary>
        /// 设置物品
        /// </summary>
        public void SetItem(ItemData item)
        {
            itemData = item;
            
            if (item == null) return;
            
            // 设置名称但不显示
            if (nameText != null)
            {
                nameText.text = item.itemName;
                nameText.enabled = false; // 默认不显示名称
            }
        }
        
        /// <summary>
        /// 处理点击事件
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 左键点击显示物品信息
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ShowItemInfo();
            }
            // 右键点击使用物品（如果可用）
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                UseItem();
            }
        }
        
        /// <summary>
        /// 处理鼠标进入事件
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 放大效果
            transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);
            
            // 显示物品名称
            if (nameText != null)
            {
                nameText.enabled = true;
            }
            
            // 高亮图标
            // 移除高亮图标代码
            /*
            if (iconImage != null)
            {
                iconImage.color = hoverColor;
            }
            */
        }
        
        /// <summary>
        /// 处理鼠标离开事件
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 恢复正常大小
            transform.localScale = Vector3.one;
            
            // 隐藏物品名称
            if (nameText != null)
            {
                nameText.enabled = false;
            }
            
            // 恢复正常颜色
            // 移除恢复颜色代码
            /*
            if (iconImage != null)
            {
                iconImage.color = normalColor;
            }
            */
        }
        
        /// <summary>
        /// 显示物品信息
        /// </summary>
        private void ShowItemInfo()
        {
            if (itemData == null || itemInfoUI == null) return;
            
            itemInfoUI.ShowItemInfo(itemData);
        }
        
        /// <summary>
        /// 使用物品
        /// </summary>
        private void UseItem()
        {
            if (itemData == null) return;
            
            // 根据物品类型执行不同的操作
            switch (itemData.itemType)
            {
                case ItemData.ItemType.Consumable:
                    Debug.Log($"使用消耗品: {itemData.itemName}");
                    // 这里可以添加消耗品的使用逻辑
                    // 使用后从库存中移除 - 暂时跳过移除逻辑
                    Debug.LogWarning($"背包系统已更改，跳过移除物品[{itemData.itemName}]的逻辑");
                    break;
                    
                case ItemData.ItemType.Readable:
                    Debug.Log($"阅读物品: {itemData.itemName}");
                    // 显示物品信息
                    ShowItemInfo();
                    break;
                    
                default:
                    // 对于其他类型的物品，只显示信息
                    ShowItemInfo();
                    break;
            }
        }
    }
}