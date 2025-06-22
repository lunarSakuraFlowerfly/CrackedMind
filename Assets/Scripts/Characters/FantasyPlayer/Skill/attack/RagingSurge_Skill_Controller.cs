using System;
using UnityEngine;

public class RagingSurge_Skill_Controller : MonoBehaviour
{
    private int attackDamageBonus;
    private int moveSpeedBonus;
    
    private PlayerStats playerStats;
    private Player player;
    private SpriteRenderer effectRenderer;
    
    private float originalMoveSpeed;
    private bool effectsApplied = false;
    
    
    /// <summary>
    /// 设置愤怒之涌效果
    /// </summary>
    public void SetupRagingSurge(int _attackDamageBonus, int _moveSpeedBonus)
    {
        attackDamageBonus = _attackDamageBonus;
        moveSpeedBonus = _moveSpeedBonus;
        player=PlayerManager.instance.player;
        playerStats=player.GetComponent<PlayerStats>();
        effectRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMoveSpeed = player.GetMoveSpeed();
        ApplyRagingSurgeEffects();
    }
    
    /// <summary>
    /// 应用愤怒之涌效果
    /// </summary>
    private void ApplyRagingSurgeEffects()
    {
        if(effectsApplied) return;
        
        if(playerStats != null)
        {
            // 增加攻击力
            playerStats.damage.AddModifier(attackDamageBonus);
            Debug.Log($"攻击力增加: +{attackDamageBonus}");
        }
        
        if(player != null)
        {
            // 增加移动速度
            player.Stats.moveSpeedPercentage.AddModifier(moveSpeedBonus);
            Debug.Log($"移动速度增加: +{moveSpeedBonus}");
        }
        
        effectsApplied = true;
    }
    
    /// <summary>
    /// 移除愤怒之涌效果
    /// </summary>
    private void RemoveRagingSurgeEffects()
    {
        Debug.Log("移除愤怒之涌效果");
        if(!effectsApplied) return;
        
        if(playerStats != null)
        {
            // 移除攻击力加成
            playerStats.damage.RemoveModifier(attackDamageBonus);
        }
        
        if(player != null)
        {
            // 恢复移动速度
            player.Stats.moveSpeedPercentage.RemoveModifier(Mathf.RoundToInt(moveSpeedBonus));
        }
        
        effectsApplied = false;
    }
    
    /// <summary>
    /// 结束愤怒之涌效果
    /// </summary>
    public void EndRagingSurge()
    {
        RemoveRagingSurgeEffects();
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        // 确保在销毁时移除效果
        if(effectsApplied)
        {
            RemoveRagingSurgeEffects();
        }
    }
}