using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunState : EnemyState
{
    private Enemy_Skeleton enemy;
    public SkeletonStunState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(8,enemy.transform);
        enemy.fx.InvokeRepeating("RedColorBlink",0,0.1f);
        stateTimer = enemy.stunDuration;
        enemy.rb.velocity = new Vector2(-enemy.stunDirection.x*enemy.facingDirection,enemy.stunDirection.y);
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("取消颜色变化");
        enemy.fx.Invoke("CancelColorChange",0);
    }
    
    

}
