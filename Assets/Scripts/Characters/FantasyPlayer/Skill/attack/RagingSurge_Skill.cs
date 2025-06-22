using UnityEngine;
using UnityEngine.UI;

public class RagingSurge_Skill : BaseSkill
{
    [Header("愤怒之涌技能设置")]
    [SerializeField] private float activationThreshold = 0.3f; // 激活阈值（30%）
    [SerializeField] private float deactivationThreshold = 0.5f; // 关闭阈值（50%）
    [SerializeField] private GameObject ragingEffectPrefab; // 特效预制体
    
    [Header("属性加成")]
    [SerializeField] private float attackDamageMultiplier = 1.5f; // 攻击力倍数
    [SerializeField] private float moveSpeedMultiplier = 1.3f; // 移动速度倍数
    
    [Header("技能解锁")]
    public bool ragingSurgeUnlocked;
    [SerializeField] private UI_SkillTreeSlot unlockButton;
    
    private bool isRagingSurgeActive = false; // 技能是否激活中
    private RagingSurge_Skill_Controller activeController; // 当前激活的控制器
    
    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Passive;
        if(unlockButton != null)
        {
            unlockButton.GetComponent<Button>().onClick.AddListener(UnlockRagingSurge);
            unlockButton.associatedSkill = this;
        }
    }
    
    protected override void Update()
    {
        base.Update();

        if(!ragingSurgeUnlocked || player == null) return;

        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        if(playerStats == null) return;

        float currentHealthPercentage = (float)playerStats.GetCurrentHP() / playerStats.GetMaxHP();

        // 检查是否需要激活技能
        if(!isRagingSurgeActive && currentHealthPercentage <= activationThreshold)
        {
            ActivateRagingSurge();
        }
        // 检查是否需要关闭技能
        else if(isRagingSurgeActive && currentHealthPercentage >= deactivationThreshold)
        {
            DeactivateRagingSurge();
        }
    }
    
    /// <summary>
    /// 激活愤怒之涌效果
    /// </summary>
    private void ActivateRagingSurge()
    {
        if(isRagingSurgeActive) return; // 防止重复激活
        
        // 计算技能效果数值
        var skillEffects = CalculateSkillEffects();
        
        // 创建技能效果对象
        GameObject ragingEffect = Instantiate(ragingEffectPrefab, player.transform.position, Quaternion.identity);
        ragingEffect.transform.SetParent(player.transform);
        ragingEffect.transform.localPosition = Vector3.zero;
        
        // 设置技能控制器
        activeController = ragingEffect.GetComponent<RagingSurge_Skill_Controller>();
        activeController.SetupRagingSurge(
            skillEffects.attackDamageBonus,
            skillEffects.moveSpeedBonus
        );
        
        isRagingSurgeActive = true;
        
    }
    
    /// <summary>
    /// 关闭愤怒之涌效果
    /// </summary>
    private void DeactivateRagingSurge()
    {
        if(!isRagingSurgeActive || activeController == null) return;
        
        activeController.EndRagingSurge();
        isRagingSurgeActive = false;
        activeController = null;
        
        Debug.Log("愤怒之涌效果结束");
    }

    public override void UseSkill()
    {
        // 这个技能现在是自动触发的，不需要手动使用
        // 但保留接口以防其他地方调用
    }
    
    /// <summary>
    /// 计算技能效果数值
    /// </summary>
    private (int attackDamageBonus, int moveSpeedBonus) CalculateSkillEffects()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if(playerStats == null)
        {
            Debug.LogError("玩家没有PlayerStats组件");
            return (0, 0);
        }
        
        // 基于角色属性计算攻击力加成
        int baseAttackDamage = playerStats.damage.GetValue();
        int strengthBonus = playerStats.strength.GetValue();
        int agilityBonus = playerStats.agility.GetValue();
        
        // 攻击力加成 = (基础攻击力 + 力量加成) * 倍数
        int attackDamageBonus = Mathf.RoundToInt((baseAttackDamage + strengthBonus) * (attackDamageMultiplier - 1));
        
        // 移动速度加成 = (基础移动速度 + 敏捷加成) * 倍数 - 基础移动速度
        float baseMoveSpeed = player.GetMoveSpeed();
        int moveSpeedBonus = Mathf.RoundToInt((baseMoveSpeed + agilityBonus *5f+100) * (moveSpeedMultiplier - 1));
        
        return (attackDamageBonus, moveSpeedBonus);
    }
    
    private void UnlockRagingSurge()
    {
        if(unlockButton != null && unlockButton.unlocked)
        {
            ragingSurgeUnlocked = true;
            Debug.Log("愤怒之涌技能已解锁");
        }
    }
    
    /// <summary>
    /// 获取技能是否激活中（用于UI显示等）
    /// </summary>
    public bool IsRagingSurgeActive()
    {
        return isRagingSurgeActive;
    }
    
    /// <summary>
    /// 强制结束技能（用于特殊情况）
    /// </summary>
    public void ForceEndRagingSurge()
    {
        if(isRagingSurgeActive)
        {
            DeactivateRagingSurge();
        }
    }

    /// <summary>
    /// 获取当前血量百分比
    /// </summary>
    public float GetCurrentHealthPercentage()
    {
        if(player == null) return 1f;
        
        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        if(playerStats == null) return 1f;
        
        return (float)playerStats.GetCurrentHP() / playerStats.GetMaxHP();
    }
    protected override void CheckUnlock()
    {
        UnlockRagingSurge();
    }
}