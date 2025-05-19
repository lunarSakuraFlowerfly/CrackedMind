using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHitedState : EnemyState
{
    private Enemy_Skeleton enemy;
    public SkeletonHitedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animatorBoolName,Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animatorBoolName)
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
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    

}
