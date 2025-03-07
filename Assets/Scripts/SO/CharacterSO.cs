using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterSO : ScriptableObject
{
    public int id;
    public string Name;
    public Property currentHp;
    public Property maxHp;
    public Property mentalValue; //¿Ì÷«
    //public Property currentMagic;
    //public Property maxMagic;
    //public Property currentHungry;
    //public Property maxHungry;
    public Property attackValue;
    public Property attackSpeed;
    public Property defensiveValue;
    public Property walkSpeed;
    public Property runSpeed;
    public Property luckyValue;
    //public Property criticalChance;
    //public Property criticalEffect;
    public Property exp;
    public Property level;
    public Property soberValue;
    public Property soberChangeSpeed;
    public Sprite iconSprite;
    public GameObject characterPrefab;
}
