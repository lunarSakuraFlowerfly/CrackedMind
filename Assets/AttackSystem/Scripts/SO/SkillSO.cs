using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkillSO : ScriptableObject
{
    [Header("分支A:勇气之力")]
    public Skill HeartSlash;  //A1心斩,主动
    public Skill RagingSurge; //A2怒燃,被动
    public Skill HeroicDash; //A3英勇猛进,主动
    public Skill DawnBreaker; //A4破晓重击,主动
    public Skill WrathfulHeart; //A5愤怒之心，被动
    public Skill DestructiveRoar; //A6毁灭咆哮，主动
    [Header("分支B:坚韧之心")]
    public Skill MindShield; //B1心灵屏障，主动
    public Skill SelfMeditation;//B2自愈冥想，主动
    public Skill FortressOfWill;//B3坚毅壁垒，被动
    public Skill TranquilWind;//B4静思之风，主动
    public Skill AegisOfLight;//B5光明庇护，主动
    [Header("分支C:意念之域")]
    public Skill BlinkStep;//C1瞬步，主动
    public Skill FearScream;//C2惊骇尖啸，主动
    public Skill PhantomSlash;//C3虚影斩，主动
    public Skill MindShock;//C4心灵震荡，主动
    public Skill BreakTheMind;//C5破心之境，主动
}
