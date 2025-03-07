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
    //MagicValue, //����
    //HungryValue, //����
    MentalValue, //����������ֵ
    MoveSpeed,
    AttackValue,
    AttackSpeed,
    DefensiveValue,//������/��־��
    Lucky, //����ֵ
    //CriticalChance, //������
    //CriticalEffect, //����Ч��
    //Hemophagia, //��Ѫ
    Exp, //����
    Level, //�ȼ�
    Sober, //���Ѷ�
    SoberChangeSpeed, //���Ѷ���������
}
