using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonWalkState : EnemyGroundState
{
    
    public SkeletonWalkState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName,_enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(enemy.GetMoveSpeed()*enemy.facingDirection,enemy.rb.velocity.y);

        if(!enemy.isGroundedDetected() || enemy.isWallDetected())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
        if(enemy.PlayerDetected()&&playerTF)
        {
            float distance = Vector2.Distance(new Vector2(playerTF.position.x,0),new Vector2(enemy.transform.position.x,0));
            if(distance>enemy.attackScale)
            {
                enemy.SetVelocity(enemy.GetMoveSpeed()*enemy.facingDirection*2,enemy.rb.velocity.y);
            }
            else if(enemy.AttackCoolDownTimer < 0)
            {
                stateMachine.ChangeState(enemy.attackState);
            }
            else if(distance<enemy.attackScale)
            {
                stateMachine.ChangeState(enemy.idleState);
            }

        }
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    

}
