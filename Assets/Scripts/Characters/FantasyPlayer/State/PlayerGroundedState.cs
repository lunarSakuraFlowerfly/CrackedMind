using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }

    
    public override void Enter()
    {
        base.Enter();
        
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.Q) && player.skill.parrySkill.parryUnlocked)
        {
            stateMachine.ChangeState(player.counterAttackState);
        }
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            stateMachine.ChangeState(player.primaryAttack);
        }
        if(Input.GetKeyDown(KeyCode.Mouse1)&&player.skill.swordSkill.CanUseSkill())
        {
            stateMachine.ChangeState(player.aimSwordState);
        }
        if(!player.isGroundedDetected())
        {
            stateMachine.ChangeState(player.airState);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.jumpState);
        }
        

    }
    private bool HasNoSword()
    {
        if(!player.Sword)
            return true;
        player.Sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}