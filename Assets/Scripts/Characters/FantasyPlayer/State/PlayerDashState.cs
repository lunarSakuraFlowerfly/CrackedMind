using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();
        player.skill.dashSkill.CloneOnDash();
        stateTimer = player.dashDuration;
        
    }
    public override void Exit()
    {
        base.Exit();
        player.skill.dashSkill.CloneOnArrival();
    }
    public override void Update()
    {
        base.Update();

        if(!player.isGroundedDetected()&&player.isWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
        player.SetVelocity(player.dashSpeed * player.facingDirection,0);
        if(stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

    }
}