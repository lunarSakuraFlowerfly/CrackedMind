using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomAttack_Skill_Controller : MonoBehaviour
{
    [Header("技能参数")]
    private float dashDuration;
    private int dashSpeed;
    private float bonusByStrength;
    private float bonusByAgility;
    private float bonusByIntelligence;
    private float bonusByVitality;
    private float bonusByDamage;
    private float magicDamage;
    
    [Header("动画控制")]
    [SerializeField] private float prepareDuration = 0.2f; // 准备阶段时间
    [SerializeField] private float attackDuration = 0.3f; // 攻击阶段时间
    
    [Header("碰撞检测")]
    [SerializeField] private float detectionRadius = 1f; // 检测半径
    [SerializeField] private LayerMask enemyLayerMask = -1; // 敌人层级
    
    // 组件引用
    private Animator effectAnimator;
    private Player player;
    private PlayerPhantomAttack phantomAttackState;
    
    // 状态控制
    private enum PhantomPhase
    {
        Preparing,  // 准备阶段
        Dashing,    // 冲刺阶段
        Attacking,  // 攻击阶段
        Ending      // 结束阶段
    }
    
    private PhantomPhase currentPhase;
    private float phaseTimer;
    private bool hasHitEnemy = false;
    private Enemy hitEnemy;
    
    private void Awake()
    {
        effectAnimator = GetComponentInChildren<Animator>();
        player = PlayerManager.instance.player;
    }
    
    public void Setup(float _dashDuration, int _dashSpeed, float _bonusByStrength, 
                     float _bonusByAgility, float _bonusByIntelligence, float _bonusByVitality, 
                     float _bonusByDamage, float _magicDamage, PlayerPhantomAttack _phantomAttackState)
    {
        dashDuration = _dashDuration;
        dashSpeed = _dashSpeed;
        bonusByStrength = _bonusByStrength;
        bonusByAgility = _bonusByAgility;
        bonusByIntelligence = _bonusByIntelligence;
        bonusByVitality = _bonusByVitality;
        bonusByDamage = _bonusByDamage;
        magicDamage = _magicDamage;
        
        phantomAttackState = _phantomAttackState;
        
        if(phantomAttackState == null)
        {
            Debug.LogError("PhantomAttackState 引用为空！");
            return;
        }
        
        StartPhantomAttack();
    }
    
    private void StartPhantomAttack()
    {
        currentPhase = PhantomPhase.Preparing;
        phaseTimer = prepareDuration;
        hasHitEnemy = false;
        
        phantomAttackState.SetPhase(PlayerPhantomAttack.PhantomPhase.Preparing);
        
        Debug.Log("幻影攻击开始 - 准备阶段");
    }
    
    private void Update()
    {
        phaseTimer -= Time.deltaTime;
        
        switch(currentPhase)
        {
            case PhantomPhase.Preparing:
                HandlePreparePhase();
                break;
            case PhantomPhase.Dashing:
                HandleDashPhase();
                break;
            case PhantomPhase.Attacking:
                HandleAttackPhase();
                break;
            case PhantomPhase.Ending:
                HandleEndPhase();
                break;
        }
    }
    
    private void HandlePreparePhase()
    {
        if(phaseTimer <= 0)
        {
            TransitionToDash();
        }
    }
    
    private void HandleDashPhase()
    {
        CheckForEnemies();
        
        if(hasHitEnemy)
        {
            TransitionToAttack();
        }
        else if(phaseTimer <= 0)
        {
            TransitionToEnd();
        }
    }
    
    private void HandleAttackPhase()
    {
        if(phaseTimer <= 0)
        {
            TransitionToEnd();
        }
    }
    
    private void HandleEndPhase()
    {
        if(phaseTimer <= 0)
        {
            EndPhantomAttack();
        }
    }
    
    private void TransitionToDash()
    {
        currentPhase = PhantomPhase.Dashing;
        phaseTimer = dashDuration;
        
        if(effectAnimator != null)
        {
            effectAnimator.SetTrigger("Dash");
        }
        
        phantomAttackState.SetPhase(PlayerPhantomAttack.PhantomPhase.Dashing);
        phantomAttackState.SetDashSpeed(dashSpeed);
        
        Debug.Log("幻影攻击 - 冲刺阶段开始");
    }
    
    private void TransitionToAttack()
    {
        currentPhase = PhantomPhase.Attacking;
        phaseTimer = attackDuration;
        
        if(effectAnimator != null)
        {
            effectAnimator.SetTrigger("Attack");
        }
        
        phantomAttackState.SetPhase(PlayerPhantomAttack.PhantomPhase.Attacking);
        
        if(hitEnemy != null)
        {
            DealDamageToEnemy(hitEnemy);
        }
        
        Debug.Log("幻影攻击 - 攻击阶段");
    }
    
    private void TransitionToEnd()
    {
        currentPhase = PhantomPhase.Ending;
        phaseTimer = 0.2f;
        
        if(effectAnimator != null)
        {
            effectAnimator.SetTrigger("End");
        }
        
        phantomAttackState.SetPhase(PlayerPhantomAttack.PhantomPhase.Ending);
        
        Debug.Log("幻影攻击 - 结束阶段");
    }
    
    private void EndPhantomAttack()
    {
        phantomAttackState.EndPhantomAttack();
        
        Destroy(gameObject);
        
        Debug.Log("幻影攻击完全结束");
    }
    
    private void CheckForEnemies()
    {
        if(hasHitEnemy) return;
        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, detectionRadius, enemyLayerMask);
        
        foreach(Collider2D col in enemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if(enemy != null)
            {
                hasHitEnemy = true;
                hitEnemy = enemy;
                break;
            }
        }
    }
    
    private void DealDamageToEnemy(Enemy enemy)
    {
        int finalDamage = CalculateDamage();
        
        enemy.Stats.TakeDamage(finalDamage);
        
        Debug.Log($"幻影攻击命中 {enemy.name}，造成 {finalDamage} 点伤害");
    }
    
    private int CalculateDamage()
    {
        int damage = 0;
        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        
        damage += (int)(playerStats.strength.GetValue() * bonusByStrength);
        damage += (int)(playerStats.agility.GetValue() * bonusByAgility);
        damage += (int)(playerStats.intelligence.GetValue() * bonusByIntelligence);
        damage += (int)(playerStats.vitality.GetValue() * bonusByVitality);
        damage += (int)(playerStats.damage.GetValue() * bonusByDamage);
        damage += (int)(playerStats.iceDamage.GetValue() * magicDamage);
        damage += (int)(playerStats.lightningDamage.GetValue() * magicDamage);
        damage += (int)(playerStats.fireDamage.GetValue() * magicDamage);
        
        return damage;
    }
    
    private void OnDrawGizmos()
    {
        if(player != null && currentPhase == PhantomPhase.Dashing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, detectionRadius);
        }
    }
}