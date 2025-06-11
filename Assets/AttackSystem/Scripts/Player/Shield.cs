using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject writeShieldPrefab; //????????»…?????????????????????çﬁ???UI????????
    public Transform playerController;
    Dictionary<string, ShieldProperty> shieldDictionary = new Dictionary<string, ShieldProperty>(); //?õ•????
    Dictionary<string, Coroutine> shieldCoroutines = new Dictionary<string, Coroutine>(); //?õ•????ßø??
    //TODO:??????UI
    void AddShield(string shieldName, ShieldProperty shieldProperty)
    {
        GameObject shieldInstance = null;
        if (shieldDictionary.ContainsKey(shieldName)) //?????????????????
        {
            shieldDictionary[shieldName].shieldValue = Mathf.Max(shieldProperty.shieldValue, shieldDictionary[shieldName].shieldValue);
        }
        else //????????????
        {
            switch (shieldName) //???????UI
            {
                case "MindShield":
                    shieldInstance = Instantiate(writeShieldPrefab, playerController);
                    shieldProperty.shieldInstance = shieldInstance;
                    break;
            }
            shieldDictionary.Add(shieldName, shieldProperty);
        }

        if (shieldCoroutines.ContainsKey(shieldName)) //????ß’??????????????????????????????????
        {
            StopCoroutine(shieldCoroutines[shieldName]);
            shieldCoroutines.Remove(shieldName);
        }
        Coroutine coroutine = StartCoroutine(RemoveShieldDelayed(shieldName)); //??????????????
        shieldCoroutines.Add(shieldName, coroutine);//??????????

    }
    /// <summary>
    /// ???????????
    /// </summary>
    /// <param name="shieldName"></param>
    private void RemoveShield(string shieldName)
    {
        
        //???????????
        if (shieldDictionary[shieldName].shieldInstance != null)
        {
            Destroy(shieldDictionary[shieldName].shieldInstance);
            Debug.Log("???????????");
            if (shieldDictionary.ContainsKey(shieldName))
                shieldDictionary.Remove(shieldName);
            if (shieldCoroutines.ContainsKey(shieldName))
            {
                StopCoroutine (shieldCoroutines[shieldName]); //??ßø????????????????
                shieldCoroutines.Remove(shieldName);
            }
        }
    }
    /// <summary>
    /// ??????????
    /// </summary>
    /// <param name="shieldName"></param>
    /// <returns></returns>
    IEnumerator RemoveShieldDelayed(string shieldName)
    {
        //???????????
        yield return new WaitForSeconds(shieldDictionary[shieldName].shieldTimer);
        if (shieldDictionary[shieldName].shieldInstance != null)
        {
            Destroy(shieldDictionary[shieldName].shieldInstance);
            Debug.Log("???????????");
        }
        if (shieldDictionary.ContainsKey(shieldName))
            shieldDictionary.Remove(shieldName);
        if (shieldCoroutines.ContainsKey(shieldName))
        {
            shieldCoroutines.Remove(shieldName);
        }
    }

    public void ChangeShieldValue(float changeValue = 0)
    {
        var keys = shieldDictionary.Keys.ToList();
        foreach(var key in keys) 
        {
            if (!shieldDictionary.ContainsKey(key))
                continue;
            var shield = shieldDictionary[key];
            if(shield == null)
                continue;

            Debug.Log("?????????10");
            shield.shieldValue = Mathf.Clamp(shield.shieldValue + changeValue, 0, float.PositiveInfinity); //0-??????
            if(changeValue > 0)
            {
                changeValue = 0;
                break;
            }
            if(changeValue < 0)
            {
                changeValue = Mathf.Clamp(changeValue + shield.shieldValue, float.NegativeInfinity, 0);
            }
            
            Debug.Log(key + ":" +  shield.shieldValue);

            if (Mathf.Approximately(shield.shieldValue, 0))
                RemoveShield(key);
            if (Mathf.Approximately(changeValue, 0))
                break;
        }
    }

    #region MindShield
    public void MindShield()
    {
        Debug.Log("????MindShield????ßπ??");
        float shieldValue = 0;
        float shieldTimer = 0;
        switch (PlayerController.Instance.playerSkillSO.HeartSlash.skillLevel)
        {
            case 1:
                shieldValue = PlayerController.Instance.playerSO.defensiveValue.value * 2f;
                shieldTimer = 5;
                break;
            case 2:
                shieldValue = PlayerController.Instance.playerSO.defensiveValue.value * 2.2f;
                shieldTimer = 5;
                break;
            case 3:
                shieldValue = PlayerController.Instance.playerSO.defensiveValue.value * 2.5f;
                shieldTimer = 5;
                break;
            case 4:
                shieldValue = PlayerController.Instance.playerSO.defensiveValue.value * 2.8f;
                shieldTimer = 6;
                break;
            case 5:
                shieldValue = PlayerController.Instance.playerSO.defensiveValue.value * 3f;
                shieldTimer = 6;
                break;
        }
        ShieldProperty shieldProperty = new ShieldProperty(shieldTimer, shieldValue,null);
        AddShield("MindShield",shieldProperty);

    }
    #endregion

}

public class ShieldProperty
{
    public float shieldTimer;
    public float shieldValue;
    public GameObject shieldInstance;
    public ShieldProperty(float shieldTimer, float shieldValue, GameObject shieldInstance)
    {
        this.shieldValue = shieldValue;
        this.shieldTimer = shieldTimer;
        this.shieldInstance = shieldInstance;
    }
}
