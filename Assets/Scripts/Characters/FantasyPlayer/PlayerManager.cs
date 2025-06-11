using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manager类，管理玩家
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Player player;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
}
