using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{
    public int Id;
    public string Name;
    public ItemType itemType;
    public string description;
    public List<Property> propertyList;
    public Sprite icon;
    public GameObject prefab;
}

public enum ItemType
{
    Weapon,
    Consumeable
}

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

public enum PropertyType
{
    HpValue,
    //MagicValue, //蓝条
    //HungryValue, //饥饿
    MentalValue, //精神，心理阈值
    MoveSpeed,
    AttackValue,
    AttackSpeed,
    DefensiveValue,//防御力/意志力
    Lucky, //幸运值
    //CriticalChance, //暴击率
    //CriticalEffect, //暴击效果
    //Hemophagia, //吸血
    Exp, //经验
    Level, //等级
    Sober, //清醒度
    SoberChangeSpeed, //清醒度增加速率
}
