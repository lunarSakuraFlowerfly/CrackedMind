using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Blackhole_Skill : BaseSkill
{
    [SerializeField] private int attackAmounts;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float BlackholeDuration;
    [Space]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float flyHeight;

    private Blackhole_Skill_Controller currentBlackhole;
    public float FlyHeight{get{return flyHeight;}}
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }
    public override void UseSkill()
    {
        base.UseSkill();
        GameObject newBlackHole = Instantiate(blackholePrefab,player.transform.position,Quaternion.identity);
        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();
        currentBlackhole.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,attackAmounts,cloneCooldown,BlackholeDuration);
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if(!currentBlackhole)
            return false;
    
        if(currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize/2;
    }
}

