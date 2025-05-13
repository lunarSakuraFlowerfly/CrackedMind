using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }
    protected virtual void Awake()
    {
        if(instance != null &&  instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = (T)this;
    }

    public static bool IsInitialized //判断是否已经生成单例
    {
        get { return (instance != null); }
    }

    protected virtual void OnDestroy() //在销毁时清空
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
