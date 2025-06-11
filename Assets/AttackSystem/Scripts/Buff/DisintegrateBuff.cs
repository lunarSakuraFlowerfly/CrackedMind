using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractionSystem.Data;
public class DisintegrateBuff : Buff
{
    //protected override void Reset()
    //{
    //    base.Reset();
    //    if(buffData == null)
    //    {
    //        string path = "Assets/BuffData/DisintegrateBuff.asset";
    //        buffData = UnityEditor.AssetDatabase.LoadAssetAtPath<BuffData>(path);
    //        if(buffData == null)
    //        {
    //            Debug.LogError("û�ҵ���BuffData");
    //        }
    //    }
    //}

    protected override void OnEnable()
    {
        if (buffData == null)
        {
            string path = "Assets/BuffData/DisintegrateBuff.asset";
            buffData = UnityEditor.AssetDatabase.LoadAssetAtPath<BuffData>(path);
            if (buffData == null)
            {
                Debug.LogError("û�ҵ���BuffData");
                return;
            }
        }
        Apply();
    }
    protected override void Apply()
    {
        base.Apply();
        buffData.attackValue.value = PlayerController.Instance.playerSO.attackValue.value * -0.2f; //����20%������
        buffData.attackSpeed.value = PlayerController.Instance.playerSO.attackSpeed.value * -0.4f; //����20%�����ٶ�
        buffData.defensiveValue.value = PlayerController.Instance.playerSO.defensiveValue.value * -0.2f; //����20%������
        buffData.moveSpeed.value = PlayerController.Instance.playerSO.walkSpeed.value * -0.4f; //����20%�ƶ��ٶ�
        PlayerController.Instance.PropertyChange(PropertyType.AttackValue, buffData.attackValue.value);
        PlayerController.Instance.PropertyChange(PropertyType.AttackSpeed, buffData.attackSpeed.value);
        PlayerController.Instance.PropertyChange(PropertyType.DefensiveValue, buffData.defensiveValue.value);
        PlayerController.Instance.PropertyChange(PropertyType.MoveSpeed, buffData.moveSpeed.value);
    }

    protected override void Remove()
    {
        base.Remove();
        PlayerController.Instance.PropertyChange(PropertyType.AttackValue, -buffData.attackValue.value);
        PlayerController.Instance.PropertyChange(PropertyType.AttackSpeed, -buffData.attackSpeed.value);
        PlayerController.Instance.PropertyChange(PropertyType.DefensiveValue, -buffData.defensiveValue.value);
        PlayerController.Instance.PropertyChange(PropertyType.MoveSpeed, -buffData.moveSpeed.value);
    }
}
