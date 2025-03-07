using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Status : Singleton<Status>
{

    public CharacterSO playerSO;

    public void ChangePlayerMental(float changeValue)
    {
        playerSO.mentalValue.value = Mathf.Clamp(playerSO.mentalValue.value + changeValue, 0f, 1f);
        if(playerSO.mentalValue.value <= 0)
        {
            //TODO: Be
            Debug.Log("Be");
        }else if (playerSO.mentalValue.value >��0 && playerSO.mentalValue.value <= 0.2f)
        {
            //TODO: ��ʾ���Ǳ��ٱ�����Ե
            Debug.Log("���ر���buff");
            gameObject.AddComponent<DisintegrateBuff>();

        }else if(playerSO.mentalValue.value >0.2f && playerSO.mentalValue.value <= 0.5f)
        {
            if(gameObject.GetComponent<DisintegrateBuff>() != null)
            {
                Debug.Log("ɾ������buff");
                DisintegrateBuff dis = gameObject.GetComponent<DisintegrateBuff>();
                Destroy(dis);
            }
        }else if(playerSO.mentalValue.value > 0.5f)
        {
            //TODO: ����buff
            Debug.Log("��������Buff");
        }
    }
    private void Update()
    {
        //text
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangePlayerMental(-0.3f);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangePlayerMental(0.3f);
        }
    }


}
