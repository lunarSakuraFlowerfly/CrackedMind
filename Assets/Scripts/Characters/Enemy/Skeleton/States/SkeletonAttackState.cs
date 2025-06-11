using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState
{
    private Enemy_Skeleton enemy;
    private Transform player;
    public SkeletonAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        triggerCalled = true;
        enemy.AttackCoolDownTimer = enemy.AttackCoolDown;
    }

    public override void Update()
    {
        base.Update();
    

        
        
        if(!triggerCalled)
        {
            if(enemy.PlayerDetected())
            {
                player = enemy.PlayerDetected().transform;
                float distance = Vector2.Distance(new Vector2(player.position.x,0),new Vector2(enemy.transform.position.x,0));
                if(distance<enemy.attackScale)
                {
                    stateMachine.ChangeState(enemy.idleState);
                }
                else
                {
                    stateMachine.ChangeState(enemy.walkState);
                }
            }
            else
            {
                stateMachine.ChangeState(enemy.walkState);
            }
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
    
    

}