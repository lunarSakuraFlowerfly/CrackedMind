using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

//Manager类，管理玩家
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Player player;
    public int currency;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public bool HaveEnoughCurrency(int _price)
    {
        if(_price>currency)
        {
            Debug.Log("Not enough currency");
            return false;
        }
        currency -= _price;
        return true;
    }
    
}
