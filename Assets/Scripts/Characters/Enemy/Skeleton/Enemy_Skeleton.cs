using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{

    public float attackScale{get;private set;}=2;
    #region 状态
    public SkeletonIdleState idleState{get;private set;}
    public SkeletonWalkState walkState{get;private set;}
    public SkeletonAttackState attackState{get;private set;}
    public SkeletonHitedState hitedState{get;private set;}
    public SkeletonStunState stunState{get;private set;}
    public SkeletonDeadState deadState{get;private set;}
    #endregion

    protected override void Awake()
    {
        base.Awake();
        idleState = new SkeletonIdleState(stateMachine,this,"Idle",this);
        walkState = new SkeletonWalkState(stateMachine,this,"Walk",this);
        attackState = new SkeletonAttackState(stateMachine,this,"Attack",this);
        hitedState = new SkeletonHitedState(stateMachine,this,"Hited",this);
        stunState = new SkeletonStunState(stateMachine,this,"Stun",this);
        deadState = new SkeletonDeadState(stateMachine,this,"Dead",this);
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState?.Update();
    }
    public override bool CanBeStunned()
    {
        if(base.CanBeStunned())
        {
            stateMachine.ChangeState(stunState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }


}