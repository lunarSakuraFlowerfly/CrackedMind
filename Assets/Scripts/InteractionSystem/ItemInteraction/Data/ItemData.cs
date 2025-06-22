using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem.Data
{
    /// <summary>
    /// 物品数据，定义物品的基本信息
    /// </summary>
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("基本信息")]
        public string itemID; // 物品唯一ID
        public string itemName;
        [TextArea(3, 10)]
        public string description;
        
        [Header("交互设置")]
        public string interactPrompt = "按E查看";
        public bool isCollectible = false; // 是否可收集
        public string collectPrompt = "按E拾取"; // 可收集物品的提示文本
        
        [Header("分类信息")]
        public ItemType itemType;
        
        [Header("背包系统扩展")]
        public Sprite icon; // 物品图标
        public GameObject prefab; // 物品预制体
        public List<Property> propertyList = new List<Property>(); // 物品属性列表
        
        // 兼容旧系统的属性
        public int Id { get { return string.IsNullOrEmpty(itemID) ? 0 : itemID.GetHashCode(); } }
        public string Name { get { return itemName; } set { itemName = value; } }
        
        // 物品类型枚举（背包系统使用）
        public enum ItemType
        {
            Consumable,   // 消耗品 = 对应背包系统的Consumeable
            Equipment,    // 装备 = 对应背包系统的Weapon
            QuestItem,    // 任务物品
            Readable,     // 可阅读物品
            Decoration    // 装饰物
        }

        public virtual string GetInteractPrompt(){
            return interactPrompt;
        }
        public virtual void SetInteractPrompt(string prompt){
            interactPrompt = prompt;
        }
        public virtual string GetDescription(){
            return description;
        }
        public virtual void SetDescription(string desc){
            description = desc;
        }
        public virtual void OnInteract()
        {
            Debug.Log($"交互物品: {itemName}");
        }

        public virtual bool CanInteract()
        {
            return true;
        }
    }
    
    // 物品属性，用于背包系统
    [Serializable]
    public class Property
    {
        public PropertyType propertyType;
        public float value;

        public Property(PropertyType propertyType, float value)
        {
            this.propertyType = propertyType;
            this.value = value;
        }
    }
    
    // 属性类型枚举，用于背包系统
    public enum PropertyType
    {
        HpValue,
        MentalValue, // 精神值/理智值
        MoveSpeed,
        AttackValue,
        AttackSpeed,
        DefensiveValue, // 防御值/抵抗力
        Lucky, // 幸运值
        Exp, // 经验
        Level, // 等级
        Sober, // 清醒度
        SoberChangeSpeed, // 清醒度变化速度
        Shiled, // 护盾值
    }
}
