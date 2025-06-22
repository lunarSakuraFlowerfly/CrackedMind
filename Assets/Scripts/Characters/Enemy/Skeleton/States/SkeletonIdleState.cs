using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : EnemyGroundState
{

    public SkeletonIdleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName, _enemy)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();
        
        if(stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.walkState);
        }
        if(enemy.PlayerDetected())
        {
            float distance = Vector2.Distance(new Vector2(playerTF.position.x,0),new Vector2(enemy.transform.position.x,0));
            if(distance>enemy.attackScale)
            {
                stateMachine.ChangeState(enemy.walkState);
            }
            else if(enemy.AttackCoolDownTimer < 0)
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }

    }

    public override void Exit()
    {
        base.Exit();

    }
    
    

}
