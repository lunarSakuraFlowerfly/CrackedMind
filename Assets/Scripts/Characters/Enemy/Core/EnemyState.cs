using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    private string animBoolName;
    

    protected float stateTimer;
    protected bool triggerCalled;


    public EnemyState(EnemyStateMachine _stateMachine,Enemy _enemy,string _animBoolName)
    {
        stateMachine = _stateMachine;
        enemyBase = _enemy;
        animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        triggerCalled = false;
        enemyBase.anim.SetBool(animBoolName,true);
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName,false);
        enemyBase.AssignLastAnimAnimName(animBoolName);
    }
    public void AnimationFinishTrigger()=>triggerCalled = false;

    
}
