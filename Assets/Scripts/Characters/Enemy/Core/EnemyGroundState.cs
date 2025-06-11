using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundState : EnemyState
{
    protected Enemy_Skeleton enemy;
    protected Transform playerTF;
    public EnemyGroundState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if(enemy.PlayerDetected())
        {
            playerTF = enemy.PlayerDetected().transform;
            float dir = playerTF.position.x - enemy.transform.position.x;
            if(dir>0 && enemy.facingDirection == -1)
            {
                enemy.Flip();
            }
            else if(dir<0 && enemy.facingDirection == 1)
            {
                enemy.Flip();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    

}