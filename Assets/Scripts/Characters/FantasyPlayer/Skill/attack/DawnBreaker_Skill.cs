using UnityEngine;
using UnityEngine.UI;

public class Dawnbreaker_Skill : BaseSkill
{
    [Header("破晓斩技能设置")]
    [SerializeField] private GameObject dawnbreakerPrefab; // 技能预制体
    [SerializeField] private float baseRange = 3f; // 基础攻击范围
    [SerializeField] private float attackWidth = 2f; // 攻击宽度
    
    [Header("伤害设置")]
    [SerializeField] private float baseDamageMultiplier = 1.2f; // 基础伤害倍数
    [SerializeField] private float baseCritChance = 0.15f; // 基础暴击率15%
    [SerializeField] private float baseCritMultiplier = 1.5f; // 基础暴击倍数
    [SerializeField] private float knockbackForce = 10f; // 击退力度
    [SerializeField] private float healthRecoveryPercent = 0.1f; // 生命恢复百分比
    
    [Header("蓄力设置")]
    [SerializeField] private float maxChargeTime = 3f; // 最大蓄力时间
    [SerializeField] private float maxDamageBonus = 0.4f; // 最大伤害加成40%
    [SerializeField] private float maxCritBonus = 0.3f; // 最大暴击率加成30%
    [SerializeField] private float minChargeTime = 0.2f; // 最小蓄力时间（准备动作时间）
    
    [Header("蓄力视觉效果")]
    [SerializeField] private float minFlickerSpeed = 3f; // 最小闪烁速度
    [SerializeField] private float maxFlickerSpeed = 10f; // 最大闪烁速度
    [SerializeField] private Color chargeColor = Color.white; // 蓄力颜色
    [SerializeField] private Material chargeMaterial; // 蓄力时的材质
    

    
    private bool isCharging = false; // 是否正在蓄力
    private float currentChargeTime = 0f; // 当前蓄力时间
    private bool canReleaseSkill = false; // 是否可以释放技能（完成最小蓄力时间）
    private SpriteRenderer playerSpriteRenderer; // 玩家精灵渲染器
    private Color originalPlayerColor; // 玩家原始颜色
    private Material originalPlayerMaterial; // 玩家原始材质
    
    // 闪烁控制
    private float flickerTimer = 0f;
    private bool isFlickerWhite = false; // 当前是否为白色闪烁状态
    
    // 技能数据（用于传递给控制器）
    private DawnbreakerSkillData storedSkillData;
    
    [Header("技能解锁")]
    public bool dawnbreakerUnlocked;
    [SerializeField] private UI_SkillTreeSlot unlockButton;
    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Assignable;
        if(unlockButton != null)
        {
            unlockButton.GetComponent<Button>().onClick.AddListener(UnlockDawnbreaker);
            unlockButton.associatedSkill = this;
        }
        
        // 获取玩家精灵渲染器
        playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        if(playerSpriteRenderer != null)
        {
            originalPlayerColor = playerSpriteRenderer.color;
            originalPlayerMaterial = playerSpriteRenderer.material;
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 处理蓄力逻辑
        HandleCharging();
    }
    
    /// <summary>
    /// 处理蓄力逻辑
    /// </summary>
    private void HandleCharging()
    {
        if(!dawnbreakerUnlocked) return;
        
        // 开始蓄力
        if(Input.GetKeyDown(KeyCode.E) && CanUseSkill() && !isCharging)
        {
            StartCharging();
        }
        
        // 蓄力中
        if(isCharging)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);
            
            // 检查是否达到最小蓄力时间
            if(!canReleaseSkill && currentChargeTime >= minChargeTime)
            {
                canReleaseSkill = true;     
            }
            
            // 更新闪烁效果
            UpdateChargingVisualEffect();
            
            // 释放技能
            if(Input.GetKeyUp(KeyCode.E) && canReleaseSkill)
            {
                ReleaseSkill();
            }
            
            // 达到最大蓄力时间自动释放
            if(currentChargeTime >= maxChargeTime)
            {
                ReleaseSkill();
            }
        }
    }
    
    /// <summary>
    /// 开始蓄力
    /// </summary>
    private void StartCharging()
    {
        isCharging = true;
        currentChargeTime = 0f;
        canReleaseSkill = false;
        flickerTimer = 0f;
        isFlickerWhite = false;
        
        // 进入蓄力状态
        if(player.stateMachine != null)
        {
            player.stateMachine.ChangeState(player.dawnbreakerState);
        }
        
    }
    
    /// <summary>
    /// 更新蓄力视觉效果
    /// </summary>
    private void UpdateChargingVisualEffect()
    {
        if(playerSpriteRenderer == null) return;
        
        // 只有在完成准备动作后才开始闪烁
        if(!canReleaseSkill)
        {
            // 准备阶段：轻微的白色效果，使用材质切换
            float prepareIntensity = currentChargeTime / minChargeTime;
            
            if(prepareIntensity > 0.5f && chargeMaterial != null)
            {
                // 切换到蓄力材质
                playerSpriteRenderer.material = chargeMaterial;
                Color prepareColor = Color.Lerp(originalPlayerColor, chargeColor, prepareIntensity * 0.3f);
                playerSpriteRenderer.color = prepareColor;
            }
            else
            {
                // 使用原始材质
                playerSpriteRenderer.material = originalPlayerMaterial;
                playerSpriteRenderer.color = originalPlayerColor;
            }
            return;
        }
        
        // 蓄力阶段：完整的闪烁效果
        float chargeProgress = (currentChargeTime - minChargeTime) / (maxChargeTime - minChargeTime);
        float flickerSpeed = Mathf.Lerp(minFlickerSpeed, maxFlickerSpeed, chargeProgress);
        
        // 更新闪烁计时器
        flickerTimer += Time.deltaTime * flickerSpeed;
        
        // 根据计时器切换闪烁状态
        bool shouldFlicker = Mathf.Sin(flickerTimer * Mathf.PI) > 0;
        
        if(shouldFlicker != isFlickerWhite)
        {
            isFlickerWhite = shouldFlicker;
            ApplyFlickerEffect(isFlickerWhite, chargeProgress);
        }
    }
    
    /// <summary>
    /// 应用闪烁效果
    /// </summary>
    private void ApplyFlickerEffect(bool isWhite, float intensity)
    {
        if(playerSpriteRenderer == null) return;
        
        if(isWhite)
        {
            // 白色闪烁状态
            if(chargeMaterial != null)
            {
                playerSpriteRenderer.material = chargeMaterial;
            }
            // 根据蓄力强度调整白色程度
            Color flickerColor = Color.Lerp(chargeColor, Color.white, intensity);
            playerSpriteRenderer.color = flickerColor;
        }
        else
        {
            // 恢复原始状态
            playerSpriteRenderer.material = originalPlayerMaterial;
            // 稍微保持一些蓄力色彩
            Color baseColor = Color.Lerp(originalPlayerColor, chargeColor, intensity * 0.2f);
            playerSpriteRenderer.color = baseColor;
        }
    }
    
    /// <summary>
    /// 释放技能
    /// </summary>
    private void ReleaseSkill()
    {
        if(!isCharging || !canReleaseSkill) return;
        
        // 预先计算技能数据
        storedSkillData = CalculateSkillData();
        
        // 停止蓄力但不立即释放技能
        // 技能将在攻击动画的特定帧释放
        StopCharging();
        
    }
    
    /// <summary>
    /// 实际释放技能（由动画事件调用）
    /// </summary>
    public void ExecuteSkillAttack()
    {
        // 检查必要的引用
        if(player == null)
        {
            return;
        }
        
        if(dawnbreakerPrefab == null)
        {
            return;
        }
        
        // 检查 storedSkillData 是否已初始化
        if(storedSkillData.damage <= 0)
        {
            storedSkillData = CalculateSkillData();
        }
        
        // 创建技能攻击范围
        int facingDirection = (int)player.facingDirection;
        Vector3 attackPosition = player.transform.position + Vector3.right * facingDirection * (storedSkillData.range * 0.5f);
        GameObject dawnbreakerAttack = Instantiate(dawnbreakerPrefab, attackPosition, Quaternion.identity);
        
        if(dawnbreakerAttack == null)
        {
            return;
        }
        
        // 设置技能控制器
        Dawnbreaker_Skill_Controller controller = dawnbreakerAttack.GetComponent<Dawnbreaker_Skill_Controller>();
        if(controller == null)
        {
            Destroy(dawnbreakerAttack);
            return;
        }
        
        controller.SetupDawnbreaker(storedSkillData, facingDirection);
        
    }
    
    /// <summary>
    /// 停止蓄力
    /// </summary>
    private void StopCharging()
    {
        isCharging = false;
        canReleaseSkill = false;
        
        // 恢复玩家原始外观
        RestorePlayerAppearance();
        
        // 重置冷却时间
        cooldownTimer = 2f;
        
        Debug.Log($"停止蓄力，准备攻击");
    }
    
    /// <summary>
    /// 恢复玩家原始外观
    /// </summary>
    private void RestorePlayerAppearance()
    {
        if(playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = originalPlayerColor;
            playerSpriteRenderer.material = originalPlayerMaterial;
        }
        
        // 重置闪烁状态
        flickerTimer = 0f;
        isFlickerWhite = false;
    }
    
    /// <summary>
    /// 强制停止蓄力
    /// </summary>
    public void ForceStopCharging()
    {
        if(isCharging)
        {
            StopCharging();
        }
    }
    
    // ... 其他方法保持不变 ...
    
    public override void UseSkill()
    {
        //角色状态机转换
        player.stateMachine.ChangeState(player.dawnbreakerState);
        base.UseSkill();
    }
    
    private DawnbreakerSkillData CalculateSkillData()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if(playerStats == null)
        {
            Debug.LogError("玩家没有PlayerStats组件");
            return new DawnbreakerSkillData();
        }
        
        // 蓄力进度（扣除准备时间）
        float effectiveChargeTime = Mathf.Max(0, currentChargeTime - minChargeTime);
        float chargeProgress = effectiveChargeTime / (maxChargeTime - minChargeTime);
        
        // 基础伤害计算：智力 + 力量 + 体力 + 基础伤害
        float baseDamage = playerStats.intelligence.GetValue() + 
                          playerStats.strength.GetValue() + 
                          playerStats.vitality.GetValue() + 
                          playerStats.damage.GetValue();
        
        // 应用基础伤害倍数
        baseDamage *= baseDamageMultiplier;
        
        // 蓄力伤害加成
        float damageBonus = chargeProgress * maxDamageBonus;
        float finalDamage = baseDamage * (1 + damageBonus);
        
        // 暴击率计算
        float finalCritChance = baseCritChance + (chargeProgress * maxCritBonus)+playerStats.critChance.GetValue();
        finalCritChance = Mathf.Clamp01(finalCritChance);
        
        // 攻击范围随蓄力轻微增大
        float finalRange = baseRange * (1 + chargeProgress * 0.2f);
        
        return new DawnbreakerSkillData
        {
            damage = finalDamage,
            critChance = finalCritChance,
            critMultiplier = baseCritMultiplier,
            range = finalRange,
            width = attackWidth,
            knockbackForce = knockbackForce,
            healthRecoveryPercent = healthRecoveryPercent,
            chargeProgress = chargeProgress
        };
    }
    
    private void UnlockDawnbreaker()
    {
        if(unlockButton != null && unlockButton.unlocked)
        {
            dawnbreakerUnlocked = true;
            Debug.Log("破晓斩技能已解锁");
        }
    }
    
    public bool IsCharging()
    {
        return isCharging;
    }
    
    public float GetChargeProgress()
    {
        if(!isCharging) return 0f;
        
        float effectiveChargeTime = Mathf.Max(0, currentChargeTime - minChargeTime);
        return effectiveChargeTime / (maxChargeTime - minChargeTime);
    }
    
    public bool CanReleaseSkill()
    {
        return canReleaseSkill;
    }
    protected override void CheckUnlock()
    {
        UnlockDawnbreaker();
    }
}

/// <summary>
/// 破晓斩技能数据结构
/// </summary>
[System.Serializable]
public struct DawnbreakerSkillData
{
    public float damage;
    public float critChance;
    public float critMultiplier;
    public float range;
    public float width;
    public float knockbackForce;
    public float healthRecoveryPercent;
    public float chargeProgress;
}
