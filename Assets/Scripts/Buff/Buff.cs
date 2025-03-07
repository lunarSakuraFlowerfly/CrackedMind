using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public BuffData buffData;
    private float timer;

    private void Start()
    {
        timer = buffData.duration;
    }
    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }else if (timer <= 0){
            Remove();
        }
    }
    private void OnEnable()
    {
        if (buffData == null)
        {
            string path = "Assets/BuffData/DisintegrateBuff.asset";
            buffData = UnityEditor.AssetDatabase.LoadAssetAtPath<BuffData>(path);
            if (buffData == null)
            {
                Debug.LogError("√ª’“µΩ∏√BuffData");
            }
        }
        Apply();
    }

    private void OnDisable()
    {
        Remove();
    }

    //protected virtual void Reset()
    //{

    //}
    protected virtual void Apply()
    {

    }
    protected virtual void Remove()
    {

    }
}
