using UnityEngine;

public class PlayerDawnbreakerState : PlayerState
{
    private Dawnbreaker_Skill dawnbreakerSkill;
    
    // 技能阶段枚举
    private enum DawnbreakerPhase
    {
        Preparing,  // 准备阶段（蓄力）
        Attacking   // 攻击阶段
    }
    
    private DawnbreakerPhase currentPhase;
    private bool hasReleasedSkill = false;
    
    public PlayerDawnbreakerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
        : base(_player, _stateMachine, _animBoolName)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        dawnbreakerSkill = SkillManager.instance.dawnbreakerSkill;
        currentPhase = DawnbreakerPhase.Preparing;
        hasReleasedSkill = false;
        
        // 进入准备动作
        player.anim.SetBool("DawnBreaker_Charging", true);
        player.anim.SetBool("DawnBreaker_Attacking", false);
        
        // 蓄力期间停止移动
        player.ZeroVelocity();
        
        Debug.Log("进入破晓斩准备阶段");
    }
    
    public override void Update()
    {
        base.Update();
        
        // 根据当前阶段处理不同逻辑
        switch(currentPhase)
        {
            case DawnbreakerPhase.Preparing:
                HandlePreparingPhase();
                break;
            case DawnbreakerPhase.Attacking:
                HandleAttackingPhase();
                break;
        }
    }
    
    /// <summary>
    /// 处理准备阶段（蓄力阶段）
    /// </summary>
    private void HandlePreparingPhase()
    {
        // 蓄力期间保持静止
        player.ZeroVelocity();
        
        // 检查是否还在蓄力
        if(dawnbreakerSkill != null && !dawnbreakerSkill.IsCharging())
        {
            // 蓄力结束，进入攻击阶段
            TransitionToAttackPhase();
        }
        
        // 允许取消蓄力（按其他键或移动）
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift) || Mathf.Abs(xInput) > 0.1f)
        {
            CancelCharging();
        }
    }
    
    /// <summary>
    /// 处理攻击阶段
    /// </summary>
    private void HandleAttackingPhase()
    {
        // 攻击期间也保持静止
        player.ZeroVelocity();
        
        // 等待攻击动画完成
        if(triggerCalled)
        {
            // 攻击动画完成，根据当前状态决定转换到哪个状态
            TransitionAfterAttack();
        }
    }
    
    /// <summary>
    /// 攻击完成后的状态转换逻辑
    /// </summary>
    private void TransitionAfterAttack()
    {
        // 检查是否在地面上
        if(player.isGroundedDetected())
        {
            // 在地面上，根据输入决定状态
            if(Mathf.Abs(xInput) > 0.1f)
            {
                // 有移动输入，转换到移动状态
                stateMachine.ChangeState(player.moveState);
                Debug.Log("攻击完成，转换到移动状态");
            }
            else
            {
                // 没有移动输入，转换到待机状态
                stateMachine.ChangeState(player.idleState);
                Debug.Log("攻击完成，转换到待机状态");
            }
        }
        else
        {
            // 在空中，转换到空中状态
            stateMachine.ChangeState(player.airState);
            Debug.Log("攻击完成，转换到空中状态");
        }
    }
    
    /// <summary>
    /// 转换到攻击阶段
    /// </summary>
    private void TransitionToAttackPhase()
    {
        currentPhase = DawnbreakerPhase.Attacking;
        hasReleasedSkill = true;
        
        // 切换到攻击动画
        player.anim.SetBool("DawnBreaker_Charging", false);
        player.anim.SetBool("DawnBreaker_Attacking", true);
        
        // 重置动画触发器
        triggerCalled = false;
        
        Debug.Log("进入破晓斩攻击阶段");
    }
    
    public override void Exit()
    {
        base.Exit();
        
        // 重置动画状态
        player.anim.SetBool("DawnBreaker_Charging", false);
        player.anim.SetBool("DawnBreaker_Attacking", false);
        
        // 确保停止蓄力
        if(dawnbreakerSkill != null && dawnbreakerSkill.IsCharging())
        {
            dawnbreakerSkill.ForceStopCharging();
        }
        
        Debug.Log("退出破晓斩状态");
    }
    
    /// <summary>
    /// 取消蓄力
    /// </summary>
    private void CancelCharging()
    {
        
        if(dawnbreakerSkill != null)
        {
            dawnbreakerSkill.ForceStopCharging();
        }
        
        // 取消时也根据当前状态决定转换
        TransitionAfterCancel();
    }
    
    /// <summary>
    /// 取消蓄力后的状态转换逻辑
    /// </summary>
    private void TransitionAfterCancel()
    {
        if(player.isGroundedDetected())
        {
            if(Mathf.Abs(xInput) > 0.1f)
            {
                stateMachine.ChangeState(player.moveState);
                Debug.Log("破晓斩蓄力被取消，转换到移动状态");
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
                Debug.Log("破晓斩蓄力被取消，转换到待机状态");
            }
        }
        else
        {
            stateMachine.ChangeState(player.airState);
            Debug.Log("破晓斩蓄力被取消，转换到空中状态");
        }
    }
    
    /// <summary>
    /// 获取当前是否在准备阶段
    /// </summary>
    public bool IsInPreparingPhase()
    {
        return currentPhase == DawnbreakerPhase.Preparing;
    }
    
    /// <summary>
    /// 获取当前是否在攻击阶段
    /// </summary>
    public bool IsInAttackingPhase()
    {
        return currentPhase == DawnbreakerPhase.Attacking;
    }
}
