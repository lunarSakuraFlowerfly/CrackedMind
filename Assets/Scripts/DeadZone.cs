using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡区域 - 当玩家进入时减少20%血量并传送到最近的检查点
/// </summary>
public class DeadZone : MonoBehaviour
{
    [Header("死亡区域设置")]
    [SerializeField] private float damagePercentage = 20f; // 伤害百分比
    [SerializeField] private bool canTriggerMultipleTimes = false; // 是否可以多次触发
    [SerializeField] private float cooldownTime = 2f; // 冷却时间，防止重复触发
    
    private bool isOnCooldown = false;
    
    [Header("音效设置")]
    [SerializeField] private bool playFallSound = false; // 是否播放掉落音效
    
    private void Start()
    {
        // 确保DeadZone有Collider2D组件且设置为Trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            Debug.LogWarning("DeadZone: 自动添加了BoxCollider2D组件");
        }
        
        if (!collider.isTrigger)
        {
            collider.isTrigger = true;
            Debug.LogWarning("DeadZone: 已将Collider设置为Trigger");
        }
    }
    
    /// <summary>
    /// 检测玩家进入死亡区域
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家
        Player player = other.GetComponent<Player>();
        if (player == null) return;
        
        // 检查冷却时间
        if (isOnCooldown && !canTriggerMultipleTimes) return;
        
        // 检查玩家是否已经死亡
        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        if (playerStats == null || playerStats.isDead) return;
        
        Debug.Log("玩家进入死亡区域！");
        
        // 开始处理死亡区域效果
        StartCoroutine(HandleDeadZoneEffect(player, playerStats));
    }
    
    /// <summary>
    /// 处理死亡区域效果的协程
    /// </summary>
    private IEnumerator HandleDeadZoneEffect(Player player, CharacterStats playerStats)
    {
        // 设置冷却状态
        isOnCooldown = true;
        
        // 播放掉落音效
        if (playFallSound && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(7, null); // 使用不同的音效ID
        }
        
        // 计算伤害（当前最大血量的20%）
        int maxHP = playerStats.GetMaxHP();
        int damageAmount = Mathf.RoundToInt(maxHP * (damagePercentage / 100f));
        
        // 确保至少造成1点伤害，但不会杀死玩家（至少保留1血）
        damageAmount = Mathf.Max(1, damageAmount);
        
        // 如果伤害会杀死玩家，则只减到1血
        int currentHP = playerStats.GetCurrentHP();
        if (currentHP - damageAmount <= 0)
        {
            damageAmount = currentHP - 1;
        }
        
        Debug.Log($"死亡区域伤害: {damageAmount} (当前血量: {currentHP}/{maxHP})");
        
        // 对玩家造成伤害
        if (damageAmount > 0)
        {
            playerStats.TakeDamage(damageAmount);
        }
        
        // 等待一小段时间让伤害效果显示
        yield return new WaitForSeconds(0.5f);
        
        // 传送到最近的检查点
        TeleportToNearestCheckpoint(player);
        
        // 等待冷却时间
        yield return new WaitForSeconds(cooldownTime);
        
        // 重置冷却状态
        isOnCooldown = false;
    }
    
    /// <summary>
    /// 传送玩家到最近的检查点
    /// </summary>
    private void TeleportToNearestCheckpoint(Player player)
    {
        // 通过GameManager获取最近的检查点
        if (GameManager.Instance != null)
        {
            Vector3 checkpointPosition = GameManager.Instance.FindClosestCheckpoint().transform.position;
            if (checkpointPosition != Vector3.zero)
            {
                // 传送玩家
                player.transform.position = checkpointPosition;
                Debug.Log($"玩家已传送到检查点: {checkpointPosition}");
                
                // 播放传送音效
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(6, null); // 传送音效
                }
            }
            else
            {
                Debug.LogWarning("未找到可用的检查点！");
            }
        }
        else
        {
            Debug.LogError("GameManager.Instance 为空，无法传送到检查点！");
        }
    }
    
}
