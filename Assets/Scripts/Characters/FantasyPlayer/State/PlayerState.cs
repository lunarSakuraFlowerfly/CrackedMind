using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础状态，其定义了动画动作、逻辑运动、状态切换
public class PlayerState 
{
    protected Player player;
    protected float xInput;
    protected PlayerStateMachine stateMachine;
    protected Rigidbody2D rb;
    protected string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;



    public PlayerState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName)
    {
        player = _player;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;
    }


    //切换状态时前期数据处理
    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName,true);
        rb = player.rb;
        triggerCalled = false;
    }


    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        xInput = Input.GetAxisRaw("Horizontal");
        player.anim.SetFloat("yVelocity",rb.velocity.y);
        
        if(Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dashSkill.CanUseSkill())
        {
            if(!player.skill.dashSkill.dashUnlocked) return;
            stateMachine.ChangeState(player.dashState);
        }

    }
 

    //退出状态时恢复数据
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName,false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

    

}
