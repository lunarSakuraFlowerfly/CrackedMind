using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallJump : PlayerState
{
    private float clampDir;
    public PlayerWallJump(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        stateTimer = .4f;
        player.SetVelocity(5*-player.facingDirection,player.jumpForce);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        if(stateTimer<0)
        {
            stateMachine.ChangeState(player.airState);
        }
        if(player.isGroundedDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

    }
}