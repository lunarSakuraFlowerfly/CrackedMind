using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Dawnbreaker_Skill_Controller : MonoBehaviour
{
    private DawnbreakerSkillData skillData;
    private int facingDirection;
    private CircleCollider2D attackCollider;
    private List<GameObject> hitEnemies = new List<GameObject>();
    
    [Header("视觉效果")]
    [SerializeField] private float effectDuration = 0.5f;
    [SerializeField] private Color normalAttackColor = Color.yellow;
    [SerializeField] private Color criticalAttackColor = Color.red;
    
    [Header("击中特效设置")]
    [SerializeField] private Vector2 hitEffectRandomRange = new Vector2(0.5f, 0.5f); // 击中特效随机位置范围
    
    private SpriteRenderer effectRenderer;
    private bool hasDealtDamage = false;
    private bool isRightFacing = true; // 默认朝右
    
    private void Start()
    {
        // 获取攻击碰撞器
        attackCollider = GetComponent<CircleCollider2D>();
        effectRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // 设置为触发器
        if(attackCollider != null)
        {
            attackCollider.isTrigger = true;
        }
        
        // 销毁延时
        Destroy(gameObject, effectDuration);
    }
    
    /// <summary>
    /// 设置破晓斩参数
    /// </summary>
    public void SetupDawnbreaker(DawnbreakerSkillData _skillData, int _facingDirection)
    {
        skillData = _skillData;
        facingDirection = _facingDirection;
        
        // 设置攻击范围
        if(attackCollider != null)
        {
            attackCollider.radius = skillData.range;
        }
        
        // ✅ 根据玩家朝向调整特效方向
        SetEffectDirection(_facingDirection);
        
        // 设置视觉效果
        SetupVisualEffect();
        
    }
    
    /// <summary>
    /// 设置特效方向
    /// </summary>
    private void SetEffectDirection(int playerFacingDirection)
    {
        // 如果玩家朝左 (facingDirection = -1) 且特效当前朝右，需要翻转
        // 如果玩家朝右 (facingDirection = 1) 且特效当前朝左，需要翻转
        bool shouldFaceRight = playerFacingDirection > 0;
        
        if(shouldFaceRight != isRightFacing)
        {
            FlipEffect();
        }
    }
    
    /// <summary>
    /// 翻转特效
    /// </summary>
    private void FlipEffect()
    {
        isRightFacing = !isRightFacing;
        
        // 方法1: 使用Scale翻转 (推荐，因为不会影响子对象的world position)
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
        // 方法2: 使用Rotation翻转 (如果需要的话可以替换上面的Scale方法)
        // transform.Rotate(0, 180, 0);
        
    }
    
    /// <summary>
    /// 设置视觉效果
    /// </summary>
    private void SetupVisualEffect()
    {
        if(effectRenderer != null)
        {
            // 根据蓄力程度设置颜色强度
            float intensity = 0.5f + skillData.chargeProgress * 0.5f;
            Color effectColor = Color.Lerp(normalAttackColor, Color.white, skillData.chargeProgress);
            effectColor.a = intensity;
            effectRenderer.color = effectColor;
            
            // 设置大小
            float scale = 0.8f + skillData.chargeProgress * 0.4f;
            
            // ✅ 保持x轴的翻转状态，只调整y和z轴的scale
            Vector3 currentScale = transform.localScale;
            float xScaleSign = Mathf.Sign(currentScale.x); // 保留x轴的正负符号
            transform.localScale = new Vector3(scale * xScaleSign, scale, scale);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 避免重复攻击同一个敌人
        if(hitEnemies.Contains(other.gameObject)) return;
        
        // 检测敌人
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null)
        {
            hitEnemies.Add(other.gameObject);
            DealDamageToEnemy(enemy);
        }
    }
    
    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    private void DealDamageToEnemy(Enemy enemy)
    {
        // 计算最终伤害
        float finalDamage = skillData.damage;
        bool isCritical = false;
        
        // 暴击判定
        if(Random.value <= skillData.critChance)
        {
            isCritical = true;
            finalDamage *= skillData.critMultiplier;
            
            // 暴击恢复生命
            RecoverHealth();
            
        }

        
        // 造成伤害
        if(isCritical)
        {
            Vector2 dir =(enemy.transform.position - PlayerManager.instance.player.transform.position).normalized;
            dir = new Vector2(dir.x,Mathf.Abs(dir.y));
            enemy.Stats.TakeDamage(Mathf.RoundToInt(finalDamage),skillData.knockbackForce,dir,0.25f);
        }
        else
        {
            enemy.Stats.TakeDamage(Mathf.RoundToInt(finalDamage));
        }
        
        // 在敌人身上播放击中特效
        PlayHitEffectOnEnemy(enemy, isCritical);
        
        // 更新技能视觉效果颜色
        if(effectRenderer != null)
        {
            effectRenderer.color = isCritical ? criticalAttackColor : normalAttackColor;
        }
        
        hasDealtDamage = true;
    }
    
    /// <summary>
    /// 在敌人身上播放击中特效
    /// </summary>
    private void 
    PlayHitEffectOnEnemy(Enemy enemy, bool isCritical)
    {
        // 查找敌人的HitEffect对象
        Transform hitEffectTransform = enemy.transform.Find("HitEffect");
        
        
        if(hitEffectTransform != null)
        {
            hitEffectTransform.gameObject.SetActive(true);
            HitEffectManager hitEffectManager = hitEffectTransform.GetComponent<HitEffectManager>();  
            if(hitEffectManager != null)
            {   
                // 播放击中特效
                Color effectColor = isCritical ? criticalAttackColor : normalAttackColor;
                hitEffectManager.ShowHitEffect(0.5f,effectColor);
            }

        }

    }
    


    
    /// <summary>
    /// 恢复生命值
    /// </summary>
    private void RecoverHealth()
    {
        Player player = PlayerManager.instance.player;
        if(player != null)
        {
            CharacterStats playerStats = player.GetComponent<CharacterStats>();
            if(playerStats != null)
            {
                int maxHealth = playerStats.GetMaxHP();
                int recoveryAmount = Mathf.RoundToInt(maxHealth * skillData.healthRecoveryPercent);
                
                playerStats.IncreaseHealthBy(recoveryAmount);
                
                Debug.Log($"破晓斩暴击恢复生命值：{recoveryAmount}");
            }
        }
    }
    
    private void Update()
    {
        effectDuration -= Time.deltaTime;
        
        // 淡出效果
        if(effectRenderer != null && effectDuration > 0)
        {
            Color color = effectRenderer.color;
            color.a = effectDuration / 0.5f;
            effectRenderer.color = color;
        }
    }
    
    /// <summary>
    /// 设置击中特效随机范围
    /// </summary>
    public void SetHitEffectRandomRange(Vector2 range)
    {
        hitEffectRandomRange = range;
    }
    
    /// <summary>
    /// 获取当前朝向
    /// </summary>
    public bool IsRightFacing()
    {
        return isRightFacing;
    }
    
    /// <summary>
    /// 强制设置朝向
    /// </summary>
    public void SetFacing(bool faceRight)
    {
        if(faceRight != isRightFacing)
        {
            FlipEffect();
        }
    }

}
