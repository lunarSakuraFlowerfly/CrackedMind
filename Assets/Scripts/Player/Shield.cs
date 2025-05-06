using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject writeShieldPrefab; //护盾预制体，以后产生护盾的时候产生预制体到玩家UI子物体下
    public Transform playerController;
    Dictionary<string, ShieldProperty> shieldDictionary = new Dictionary<string, ShieldProperty>(); //存储护盾
    Dictionary<string, Coroutine> shieldCoroutines = new Dictionary<string, Coroutine>(); //存储护盾协程
    //TODO:做护盾UI
    void AddShield(string shieldName, ShieldProperty shieldProperty)
    {
        GameObject shieldInstance = null;
        if (shieldDictionary.ContainsKey(shieldName)) //查字典，有了则更新数值
        {
            shieldDictionary[shieldName].shieldValue = Mathf.Max(shieldProperty.shieldValue, shieldDictionary[shieldName].shieldValue);
        }
        else //没有则添加字典
        {
            switch (shieldName) //添加护盾UI
            {
                case "MindShield":
                    shieldInstance = Instantiate(writeShieldPrefab, playerController);
                    shieldProperty.shieldInstance = shieldInstance;
                    break;
            }
            shieldDictionary.Add(shieldName, shieldProperty);
        }

        if (shieldCoroutines.ContainsKey(shieldName)) //如果有此进程了，停止进程，并且从字典里删除这个进程
        {
            StopCoroutine(shieldCoroutines[shieldName]);
            shieldCoroutines.Remove(shieldName);
        }
        Coroutine coroutine = StartCoroutine(RemoveShieldDelayed(shieldName)); //统一重新启动进程
        shieldCoroutines.Add(shieldName, coroutine);　//添加进程字典

    }
    /// <summary>
    /// 立刻删除护盾
    /// </summary>
    /// <param name="shieldName"></param>
    private void RemoveShield(string shieldName)
    {
        if (shieldDictionary.ContainsKey(shieldName))
            shieldDictionary.Remove(shieldName);
        shieldCoroutines.Remove(shieldName);
        //删除护盾物体
        if (shieldDictionary[shieldName].shieldInstance != null)
        {
            Destroy(shieldDictionary[shieldName].shieldInstance);
            Debug.Log("删除护盾物体");
        }
    }
    /// <summary>
    /// 延时删除护盾
    /// </summary>
    /// <param name="shieldName"></param>
    /// <returns></returns>
    IEnumerator RemoveShieldDelayed(string shieldName)
    {

        yield return new WaitForSeconds(shieldDictionary[shieldName].shieldTimer);
        if (shieldDictionary[shieldName].shieldInstance != null)
        {
            Destroy(shieldDictionary[shieldName].shieldInstance);
            Debug.Log("删除护盾物体");
        }
        if (shieldDictionary.ContainsKey(shieldName))
            shieldDictionary.Remove(shieldName);
        shieldCoroutines.Remove(shieldName);
        //删除护盾物体
        
    }

    public void ChangeShieldValue(float changeValue = 0)
    {
        foreach(var shield in shieldDictionary) 
        {
            if(shield.Value != null) //shield.Value得到ShieldProperty
            {
                //TODO:这里没有成功删除护盾
                Debug.Log("减少护盾量10");
                shield.Value.shieldValue = Mathf.Clamp(shield.Value.shieldValue + changeValue, 0, float.PositiveInfinity); //0-正无穷
            }
            if (Mathf.Approximately(shield.Value.shieldValue, 0))
                RemoveShield(shield.Key);
        }
    }

    #region MindShield
    public void MindShield()
    {
        Debug.Log("触发MindShield技能效果");
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
