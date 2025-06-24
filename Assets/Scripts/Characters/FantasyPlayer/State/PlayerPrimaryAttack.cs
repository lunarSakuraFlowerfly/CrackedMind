using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{
    public int comboCounter{get;private set;}
    private float lastTimeAttacked;
    private float comboWindow = 2;

    public PlayerPrimaryAttack(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(0,null);//attack effect
        player.fx.ScreenShake();
        player.anim.SetInteger("ComboCounter",comboCounter);
        if(Time.time - lastTimeAttacked > comboWindow)
        {
            comboCounter = 0;
        }

        #region 选择攻击方向
        float attackDirection = player.facingDirection;
        
        #endregion


        player.anim.speed = player.GetAttackSpeed();
        player.SetVelocity(player.attackMovement[comboCounter].x*attackDirection,player.attackMovement[comboCounter].y);
        stateTimer = 0.1f;
    }
    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine(player.BusyFor(0.1f));
        player.anim.speed = 1;

        comboCounter=(comboCounter+1)%3;
        
        lastTimeAttacked = Time.time;
    
    }
    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            player.ZeroVelocity();
        }
        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}