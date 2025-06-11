using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InteractionSystem.Core;
using InteractionSystem.Audio;
using InteractionSystem.Data;

namespace InteractionSystem.Item
{
    /// <summary>
    /// 物品交互组件，实现物品的交互逻辑
    /// </summary>
    public class ItemInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private bool canInteract = true;
        
        [Header("音效设置")]
        [SerializeField] private bool playInteractSound = true; // 是否播放交互音效
        
        // 物品信息UI引用
        private ItemInfoUI itemInfoUI;
        
        // 物品收集事件
        public static event Action<ItemData> OnItemCollected;
        
        // Start is called before the first frame update
        void Start()
        {
            // 查找物品信息UI
            itemInfoUI = FindObjectOfType<ItemInfoUI>();
            
            if (itemInfoUI == null)
            {
                Debug.LogWarning("未找到ItemInfoUI，物品信息将无法显示");
            }
        }

        /// <summary>
        /// 获取交互提示文本
        /// </summary>
        public string GetInteractPrompt()
        {
            if (itemData == null) return "按E交互";
            return itemData.GetInteractPrompt();
        }
        
        /// <summary>
        /// 获取交互描述文本
        /// </summary>
        public string GetDescription()
        {
            return itemData.GetDescription();
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public bool CanInteract()
        {
            return canInteract && itemData != null && itemData.CanInteract();
        }
        
        /// <summary>
        /// 执行交互
        /// </summary>
        public void Interact()
        {
            if (!CanInteract()) return;
            
            // 播放交互开始音效
            if (playInteractSound && InteractionAudioManager.Instance != null)
            {
                InteractionAudioManager.Instance.PlayInteractionStartSound();
            }
            
            if (itemData.isCollectible)
            {
                // 如果是可收集物品，直接加入背包
                CollectItem();
            }
            else
            {
                // 如果是不可收集物品，显示物品信息
                ShowItemInfo();
            }
            itemData.OnInteract();
        }
        
        /// <summary>
        /// 收集物品
        /// </summary>
        private void CollectItem()
        {
            // 播放物品收集音效
            if (playInteractSound && InteractionAudioManager.Instance != null)
            {
                InteractionAudioManager.Instance.PlayItemCollectSound();
            }
            
            // 触发物品收集事件
            OnItemCollected?.Invoke(itemData);
            
            // 直接添加到背包
            if (InventoryUI.Instance != null)
            {
                InventoryUI.Instance.AddItem(itemData);
                Debug.Log($"已将物品 [{itemData.itemName}] 添加到背包");
            }
            else
            {
                Debug.LogWarning("未找到InventoryUI实例，无法添加物品到背包");
            }
            
            // 销毁物品对象
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 显示物品信息
        /// </summary>
        private void ShowItemInfo()
        {
            if (itemInfoUI != null)
            {
                itemInfoUI.ShowItemInfo(itemData);
            }
            else
            {
                // 如果没有找到UI，至少在控制台输出信息
                Debug.Log($"物品: {itemData.itemName}\n描述: {itemData.description}");
                
                // 播放交互完成音效
                if (playInteractSound && InteractionAudioManager.Instance != null)
                {
                    InteractionAudioManager.Instance.PlayInteractionCompleteSound();
                }
            }
        }
        
        /// <summary>
        /// 获取交互对象的变换组件
        /// </summary>
        public Transform GetTransform()
        {
            return transform;
        }
        
        /// <summary>
        /// 获取物品数据
        /// </summary>
        public ItemData GetItemData()
        {
            return itemData;
        }
        
        /// <summary>
        /// 设置物品数据
        /// </summary>
        public void SetItemData(ItemData newItemData)
        {
            itemData = newItemData;
        }
        
        /// <summary>
        /// 设置是否可交互
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            canInteract = interactable;
        }
    }
}
