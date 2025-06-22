using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : BaseSkill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;
    [Header("水晶镜像")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalMirrorButton;
    [SerializeField] private bool cloneInsteadOfCrystal;
    [Header("水晶样本")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked{get;private set;}
    [Header("爆炸")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosionButton;
    [SerializeField] private bool canExplode;
    [Header("移动")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;
    
    [Header("多水晶")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiCrystalButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
        RefillCrystal();
        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCrystalMirrorButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirror);
        unlockExplosionButton.GetComponent<Button>().onClick.AddListener(UnlockExplosion);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMultiCrystal);
    }
    protected override void Update()
    {
        base.Update();
    }
    #region 解锁技能
    private void UnlockCrystal()
    {
        if(unlockCrystalButton.unlocked)
        {
            crystalUnlocked = true;
        }
    }

    
    private void UnlockCrystalMirror()
    {
        if(unlockCrystalMirrorButton.unlocked)
        {
            cloneInsteadOfCrystal = true;
        }
    }

    private void UnlockExplosion()
    {
        if(unlockExplosionButton.unlocked)
        {
            canExplode = true;
        }
    }

    private void UnlockMovingCrystal()
    {
        if(unlockMovingCrystalButton.unlocked)
        {
            canMove = true;
        }
    }

    private void UnlockMultiCrystal()
    {
        if(unlockMultiCrystalButton.unlocked)
        {
            canUseMultiStacks = true;
        }
    }
    #endregion
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

