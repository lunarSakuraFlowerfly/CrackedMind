using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimatorEvents : MonoBehaviour
{
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    private void AnimationTrigger()
    {
        enemy.stateMachine.currentState.AnimationFinishTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Player>(out Player player))
            {
                enemy.Stats.DoDamage(player.Stats);
            }
        }
    }
    private void DeadTrigger()
    {
        enemy.SelfDestroy();
    }
    protected void OpenCounterAttackWindow() => enemy.OpenCounterAttackWindow();
    protected void CloseCounterAttackWindow() => enemy.CloseCounterAttackWindow();
}
