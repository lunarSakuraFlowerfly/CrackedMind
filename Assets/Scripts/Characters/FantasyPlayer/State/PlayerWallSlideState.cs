using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    private float clampDir;
    private float velocityY;
    public PlayerWallSlideState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        clampDir = player.facingDirection;
        velocityY = -player.rb.gravityScale*0.5f;
        player.SetVelocity(0,velocityY);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        player.SetVelocity(0,velocityY);
        if(player.isWallDetected()==false)
        {
            stateMachine.ChangeState(player.airState);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }
        if(player.facingDirection != clampDir)
        {
            player.Flip();
        }
        
        if(player.isGroundedDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
        
        if(xInput != 0)
        {
            if(player.facingDirection != xInput)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
        
        


    }
}