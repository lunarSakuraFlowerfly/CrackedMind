using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class SkeletonDeadState : EnemyState
{
    private Enemy_Skeleton enemy;
    public SkeletonDeadState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer=.1f;

    }

    public override void Update()
    {
        base.Update();
        
        if(stateTimer>0)
        {
            enemy.rb.velocity = new Vector2(0,10);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    

}