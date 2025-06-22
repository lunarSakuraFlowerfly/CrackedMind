using UnityEngine;
using UnityEngine.UI;

public class PhantomAttack_Skill : BaseSkill
{
    [Header("技能设置")]
    [SerializeField] private GameObject phantomAttackPrefab;
    [SerializeField] private UI_SkillTreeSlot unlockSkillButton;
    [SerializeField] private bool canUseSkill;

    [Header("冲刺参数")]
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private int dashSpeed = 20;
    
    [Header("伤害加成")]
    [SerializeField] private float BonusByStrength = 1.2f;
    [SerializeField] private float BonusByAgility = 1.5f;
    [SerializeField] private float BonusByIntelligence = 0.8f;
    [SerializeField] private float BonusByVitality = 0.5f;
    [SerializeField] private float BonusByDamage = 2f;
    [SerializeField] private float MagicDamage = 1f;
    
    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Assignable;
        if(unlockSkillButton != null)
        {
            unlockSkillButton.GetComponent<Button>().onClick.AddListener(UnlockSkill);
            unlockSkillButton.associatedSkill = this;
        }
    }
    
    protected override void Update()
    {
        base.Update();
    }
    
    public override void UseSkill()
    {
        //角色状态机转换
        player.stateMachine.ChangeState(player.phantomState);
        base.UseSkill();
        

        if(player.phantomState == null)
        {
            Debug.LogError("player.phantomState 为空！请检查Player类中的状态初始化。");
            return;
        }
        
        player.stateMachine.ChangeState(player.phantomState);
        
        GameObject phantomAttack = Instantiate(phantomAttackPrefab, player.transform.position, Quaternion.identity);
        phantomAttack.transform.SetParent(player.transform);
        if(!player.isRightFacing)
        {
            phantomAttack.transform.Rotate(0,180,0);
        }
        PhantomAttack_Skill_Controller controller = phantomAttack.GetComponent<PhantomAttack_Skill_Controller>();
        
        if(controller != null)
        {
            controller.Setup(
                dashDuration, 
                dashSpeed, 
                BonusByStrength, 
                BonusByAgility, 
                BonusByIntelligence, 
                BonusByVitality, 
                BonusByDamage, 
                MagicDamage,
                player.phantomState
            );
        }
        else
        {
            Debug.LogError("PhantomAttack_Skill_Controller 组件未找到！");
        }
        
        cooldownTimer = cooldown;
        
        Debug.Log("幻影攻击技能释放！");
    }
    
    private void UnlockSkill()
    {
        if(unlockSkillButton != null && unlockSkillButton.unlocked)
        {
            canUseSkill = true;
            Debug.Log("幻影攻击技能已解锁");
        }
    }
    
    public override bool CanUseSkill()
    {
        if(!canUseSkill) 
        {
            Debug.Log("幻影攻击技能未解锁");
            return false;
        }
        return base.CanUseSkill();
    }
    protected override void CheckUnlock()
    {
        UnlockSkill();
    }
}