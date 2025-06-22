using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public CharacterSO playerSO;
    public bool isRecording= false;
    public bool waitForRelease = false;
    public KeyCode heartSlath;
    private int branchNumber = 0; //��¼��֧����
    private int thYear = 1; //��Ŀ
    

    private void Update()
    {
        #region ������������
        if (Input.GetKeyDown(KeyCode.M)) //����
        {
            PlayerController.Instance.playerSkillSO.HeartSlash.isSkill = true;
            HeartSlashLevelUP(PlayerController.Instance.playerSkillSO);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerController.Instance.playerSkillSO.MindShield.isSkill = true;
            MindShieldLevelUP(PlayerController.Instance.playerSkillSO);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            playerSO.soulFragment += 10;
        }
        #endregion
        #region ��ݼ����ò���
        //if (Input.GetKeyDown(heartSlath))
        //{
        //    Debug.Log("�ͷż���heartSlash");
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha0) && !isRecording)
        //{
        //    isRecording = true;
        //    waitForRelease = true; //�ȴ��ͷ�Alpha0
        //    Debug.Log("��ʼ��¼��ݼ�");
        //}

        //if(waitForRelease && !Input.anyKey)
        //{
        //    waitForRelease = false;
        //    Debug.Log("��ʼ������ݼ�");
        //}
        //else if (isRecording && !waitForRelease)
        //{

        //    foreach(KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        //    {
        //        if (Input.GetKey(keyCode) && keyCode != KeyCode.Mouse0)
        //        {
        //            Debug.Log($"���¿�ݼ�{keyCode}");
        //            heartSlath = keyCode;
        //            isRecording = false;
        //            break;
        //        }
        //    }
        //}
        #endregion
    }
    public void HeartSlashLevelUP(SkillSO skillSO) //ģ��
    {
        if (!skillSO.HeartSlash.isSkill) return;
        switch (skillSO.HeartSlash.skillLevel)
        {
            case 0:
                if (playerSO.soulFragment >= 10)
                {
                    Debug.Log("HeartSlash�����ɹ�");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 10;
                    skillSO.HeartSlash.skillTime = 8;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
            case 1:
                if (playerSO.soulFragment >= 15)
                {
                    Debug.Log("HeartSlash�����ɹ�");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 15;
                    skillSO.HeartSlash.skillTime = 7.5f;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
            case 2:
                if (playerSO.soulFragment >= 20)
                {
                    Debug.Log("HeartSlash�����ɹ�");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 20;
                    skillSO.HeartSlash.skillTime = 7;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
            case 3:
                if (branchNumber < 2 || thYear >= 2)
                {
                    if (playerSO.soulFragment >= 30)
                    {
                        Debug.Log("HeartSlash�����ɹ�");
                        skillSO.HeartSlash.skillLevel += 1;
                        playerSO.soulFragment -= 30;
                        branchNumber += 1;
                        skillSO.HeartSlash.skillTime = 6.5f;
                    }
                    else Debug.Log("�����Ƭ���㣬����ʧ��");
                }
                else Debug.Log("δ�ﵽ����Ŀ����������֧����");
                
                break;
            case 4:
                if (playerSO.soulFragment >= 40)
                {
                    Debug.Log("HeartSlash�����ɹ�");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 40;
                    skillSO.HeartSlash.skillTime = 6;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
        }
    }
    public void MindShieldLevelUP(SkillSO skillSO)
    {
        if (!skillSO.MindShield.isSkill) return;
        switch (skillSO.MindShield.skillLevel)
        {
            case 0:
                if (playerSO.soulFragment >= 10)
                {
                    Debug.Log("MindShield�����ɹ�");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 10;
                    skillSO.MindShield.skillTime = 8;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
            case 1:
                if (playerSO.soulFragment >= 15)
                {
                    Debug.Log("MindShield�����ɹ�");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 15;
                    skillSO.MindShield.skillTime = 7.5f;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
            case 2:
                if (playerSO.soulFragment >= 20)
                {
                    Debug.Log("MindShield�����ɹ�");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 20;
                    skillSO.MindShield.skillTime = 7;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
            case 3:
                if (branchNumber < 2 || thYear >= 2)
                {
                    if (playerSO.soulFragment >= 30)
                    {
                        Debug.Log("MindShield�����ɹ�");
                        skillSO.MindShield.skillLevel += 1;
                        playerSO.soulFragment -= 30;
                        branchNumber += 1;
                        skillSO.MindShield.skillTime = 6.5f;
                    }
                    else Debug.Log("�����Ƭ���㣬����ʧ��");
                }
                else Debug.Log("δ�ﵽ����Ŀ����������֧����");

                break;
            case 4:
                if (playerSO.soulFragment >= 40)
                {
                    Debug.Log("MindShield�����ɹ�");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 40;
                    skillSO.MindShield.skillTime = 6;
                }
                else Debug.Log("�����Ƭ���㣬����ʧ��");
                break;
        }
    }
}

[Serializable]
public class Skill
{
    public SkillTypes skillType;
    public bool isSkill;
    public int skillLevel;
    public float skillTime;
    public KeyCode skillKey;
    public Skill(SkillTypes skillTypes, bool isSkill, int skillLevel)
    {
        this.skillType = skillTypes;
        this.isSkill = isSkill;
        this.skillLevel = skillLevel;
    }
}

public enum SkillTypes
{
    #region ��֧A:����֮��
    HeartSlash,  //A1��ն,����
    RagingSurge, //A2ŭȼ,����
    HeroicDash, //A3Ӣ���ͽ�,����
    DawnBreaker, //A4�����ػ�,����
    WrathfulHeart, //A5��ŭ֮�ģ�����
    DestructiveRoar, //A6��������������
    #endregion
    #region ��֧B:����֮��
    MindShield, //B1�������ϣ�����
    SelfMeditation,//B2����ڤ�룬����
    FortressOfWill,//B3������ݣ�����
    TranquilWind,//B4��˼֮�磬����
    AegisOfLight,//B5�����ӻ�������
    #endregion
    #region ��֧C:����֮��
    BlinkStep,//C1˲��������
    FearScream,//C2������Х������
    PhantomSlash,//C3��Ӱն������
    MindShock,//C4�����𵴣�����
    BreakTheMind,//C5����֮��������
    #endregion
}
