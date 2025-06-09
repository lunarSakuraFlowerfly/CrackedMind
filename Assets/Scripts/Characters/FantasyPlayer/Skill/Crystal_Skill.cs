using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Crystal_Skill : BaseSkill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;
    [Header("水晶镜像")]
    [SerializeField] private bool cloneInsteadOfCrystal;
    [Header("爆炸")]
    [SerializeField] private bool canExplode;
    [Header("移动")]
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;
    
    [Header("多水晶")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
        RefillCrystal();
        
    }
    protected override void Update()
    {
        base.Update();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal()) return;
        if(currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(canMove) return;
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;
            if(cloneInsteadOfCrystal)
            {
                player.skill.cloneSkill.CreateClone(currentCrystal.transform,Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }
    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab,player.transform.position,Quaternion.identity);
        currentCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration,canExplode,canMove,moveSpeed,FindClosestEnemy(currentCrystal.transform));
    }
    public void CurrentCrystalChooseRandomTarget()=>currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();
    private bool CanUseMultiCrystal()
    {

        if(canUseMultiStacks)
        {
            if(crystalLeft.Count == amountOfStacks)
            {
                Invoke("ResetAbility",useTimeWindow);
            }

            //生成水晶
            if(crystalLeft.Count > 0)
            {
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count-1];
                GameObject newCrystal = Instantiate(crystalToSpawn,player.transform.position,Quaternion.identity);
                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration,canExplode,canMove,moveSpeed,FindClosestEnemy(newCrystal.transform));
                if(crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefillCrystal();
                }

            }
        }
        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for(int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if(cooldownTimer <= 0)
        {
            cooldownTimer = multiStackCooldown;
            RefillCrystal();
        }
    }
}

