using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class Stat 
{
    [SerializeField] private int baseValue;
    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;
        foreach(int modifier in modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }

    public IEnumerator RemoveModifierCoroutine(int _modifier,float _duration)
    {
        yield return new WaitForSeconds(_duration);
        RemoveModifier(_modifier);
    }
}