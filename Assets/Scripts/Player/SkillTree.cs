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
    private int branchNumber = 0; //记录分支数量
    private int thYear = 1; //周目
    

    private void Update()
    {
        #region 技能升级测试
        if (Input.GetKeyDown(KeyCode.M)) //测试
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
        #region 快捷键设置测试
        //if (Input.GetKeyDown(heartSlath))
        //{
        //    Debug.Log("释放技能heartSlash");
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha0) && !isRecording)
        //{
        //    isRecording = true;
        //    waitForRelease = true; //等待释放Alpha0
        //    Debug.Log("开始记录快捷键");
        //}

        //if(waitForRelease && !Input.anyKey)
        //{
        //    waitForRelease = false;
        //    Debug.Log("开始监听快捷键");
        //}
        //else if (isRecording && !waitForRelease)
        //{

        //    foreach(KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        //    {
        //        if (Input.GetKey(keyCode) && keyCode != KeyCode.Mouse0)
        //        {
        //            Debug.Log($"更新快捷键{keyCode}");
        //            heartSlath = keyCode;
        //            isRecording = false;
        //            break;
        //        }
        //    }
        //}
        #endregion
    }
    public void HeartSlashLevelUP(SkillSO skillSO) //模版
    {
        if (!skillSO.HeartSlash.isSkill) return;
        switch (skillSO.HeartSlash.skillLevel)
        {
            case 0:
                if (playerSO.soulFragment >= 10)
                {
                    Debug.Log("HeartSlash升级成功");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 10;
                    skillSO.HeartSlash.skillTime = 8;
                }
                else Debug.Log("灵魂碎片不足，解锁失败");
                break;
            case 1:
                if (playerSO.soulFragment >= 15)
                {
                    Debug.Log("HeartSlash升级成功");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 15;
                    skillSO.HeartSlash.skillTime = 7.5f;
                }
                else Debug.Log("灵魂碎片不足，升级失败");
                break;
            case 2:
                if (playerSO.soulFragment >= 20)
                {
                    Debug.Log("HeartSlash升级成功");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 20;
                    skillSO.HeartSlash.skillTime = 7;
                }
                else Debug.Log("灵魂碎片不足，升级失败");
                break;
            case 3:
                if (branchNumber < 2 || thYear >= 2)
                {
                    if (playerSO.soulFragment >= 30)
                    {
                        Debug.Log("HeartSlash升级成功");
                        skillSO.HeartSlash.skillLevel += 1;
                        playerSO.soulFragment -= 30;
                        branchNumber += 1;
                        skillSO.HeartSlash.skillTime = 6.5f;
                    }
                    else Debug.Log("灵魂碎片不足，升级失败");
                }
                else Debug.Log("未达到二周目，可升级分支已满");
                
                break;
            case 4:
                if (playerSO.soulFragment >= 40)
                {
                    Debug.Log("HeartSlash升级成功");
                    skillSO.HeartSlash.skillLevel += 1;
                    playerSO.soulFragment -= 40;
                    skillSO.HeartSlash.skillTime = 6;
                }
                else Debug.Log("灵魂碎片不足，升级失败");
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
                    Debug.Log("MindShield升级成功");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 10;
                    skillSO.MindShield.skillTime = 8;
                }
                else Debug.Log("灵魂碎片不足，解锁失败");
                break;
            case 1:
                if (playerSO.soulFragment >= 15)
                {
                    Debug.Log("MindShield升级成功");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 15;
                    skillSO.MindShield.skillTime = 7.5f;
                }
                else Debug.Log("灵魂碎片不足，升级失败");
                break;
            case 2:
                if (playerSO.soulFragment >= 20)
                {
                    Debug.Log("MindShield升级成功");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 20;
                    skillSO.MindShield.skillTime = 7;
                }
                else Debug.Log("灵魂碎片不足，升级失败");
                break;
            case 3:
                if (branchNumber < 2 || thYear >= 2)
                {
                    if (playerSO.soulFragment >= 30)
                    {
                        Debug.Log("MindShield升级成功");
                        skillSO.MindShield.skillLevel += 1;
                        playerSO.soulFragment -= 30;
                        branchNumber += 1;
                        skillSO.MindShield.skillTime = 6.5f;
                    }
                    else Debug.Log("灵魂碎片不足，升级失败");
                }
                else Debug.Log("未达到二周目，可升级分支已满");

                break;
            case 4:
                if (playerSO.soulFragment >= 40)
                {
                    Debug.Log("MindShield升级成功");
                    skillSO.MindShield.skillLevel += 1;
                    playerSO.soulFragment -= 40;
                    skillSO.MindShield.skillTime = 6;
                }
                else Debug.Log("灵魂碎片不足，升级失败");
                break;
        }
    }
}

[Serializable]
public class Skill
{
    public SkillType skillType;
    public bool isSkill;
    public int skillLevel;
    public float skillTime;
    public KeyCode skillKey;
    public Skill(SkillType skillType, bool isSkill, int skillLevel)
    {
        this.skillType = skillType;
        this.isSkill = isSkill;
        this.skillLevel = skillLevel;
    }
}

public enum SkillType
{
    #region 分支A:勇气之力
    HeartSlash,  //A1心斩,主动
    RagingSurge, //A2怒燃,被动
    HeroicDash, //A3英勇猛进,主动
    DawnBreaker, //A4破晓重击,主动
    WrathfulHeart, //A5愤怒之心，被动
    DestructiveRoar, //A6毁灭咆哮，主动
    #endregion
    #region 分支B:坚韧之心
    MindShield, //B1心灵屏障，主动
    SelfMeditation,//B2自愈冥想，主动
    FortressOfWill,//B3坚毅壁垒，被动
    TranquilWind,//B4静思之风，主动
    AegisOfLight,//B5光明庇护，主动
    #endregion
    #region 分支C:意念之域
    BlinkStep,//C1瞬步，主动
    FearScream,//C2惊骇尖啸，主动
    PhantomSlash,//C3虚影斩，主动
    MindShock,//C4心灵震荡，主动
    BreakTheMind,//C5破心之境，主动
    #endregion
}
