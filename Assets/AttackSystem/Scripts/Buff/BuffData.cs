using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractionSystem.Data;

[CreateAssetMenu()]
public class BuffData : ScriptableObject
{
    public string buffName;
    public float duration; //ʱ
    public Property hpValue;
    public Property mentalValue;
    public Property attackValue;
    public Property attackSpeed;
    public Property defensiveValue;
    public Property moveSpeed;
    public Property luckyValue;
    public Property soberValue;
    public Property soberChangeSpeed;
}
