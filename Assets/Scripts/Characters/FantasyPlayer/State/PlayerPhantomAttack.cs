using UnityEngine;

public class PlayerPhantomAttack : PlayerState
{
    public enum PhantomPhase
    {
        Preparing,  // 准备阶段
        Dashing,    // 冲刺阶段
        Attacking,  // 攻击阶段
        Ending      // 结束阶段
    }
    
    private PhantomPhase currentPhase;
    private int dashSpeed;
    private bool isDashing = false;
    
    public PlayerPhantomAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
        : base(_player, _stateMachine, _animBoolName)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        currentPhase = PhantomPhase.Preparing;
        isDashing = false;
        
        // 设置准备动画
        player.anim.SetBool("PhantomPrepare", true);
        player.anim.SetBool("PhantomAttack", false);
        
        // 停止移动
        player.ZeroVelocity();
        
        Debug.Log("进入幻影攻击状态 - 准备阶段");
    }
    
    public override void Update()
    {
        base.Update();
        
        switch(currentPhase)
        {
            case PhantomPhase.Preparing:
                HandlePreparePhase();
                break;
            case PhantomPhase.Dashing:
                HandleDashPhase();
                break;
            case PhantomPhase.Attacking:
                HandleAttackPhase();
                break;
            case PhantomPhase.Ending:
                HandleEndPhase();
                break;
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        
        // 重置动画状态
        player.anim.SetBool("PhantomPrepare", false);
        player.anim.SetBool("PhantomAttack", false);
        
        // 停止冲刺
        isDashing = false;
        
        Debug.Log("退出幻影攻击状态");
    }
    
    private void HandlePreparePhase()
    {
        // 准备阶段保持静止
        player.ZeroVelocity();
    }
    
    private void HandleDashPhase()
    {
        if(isDashing)
        {
            // 冲刺移动
            float dashVelocity = dashSpeed * player.facingDirection;
            player.SetVelocity(dashVelocity, rb.velocity.y);
        }
    }
    
    private void HandleAttackPhase()
    {
        // 攻击阶段停止移动
        player.ZeroVelocity();
        
        // 等待攻击动画完成
        if(triggerCalled)
        {
            // 攻击动画完成，可以准备结束
        }
    }
    
    private void HandleEndPhase()
    {
        // 结束阶段停止移动
        player.ZeroVelocity();
    }
    
    /// <summary>
    /// 设置当前阶段
    /// </summary>
    public void SetPhase(PhantomPhase newPhase)
    {
        currentPhase = newPhase;
        
        switch(newPhase)
        {
            case PhantomPhase.Preparing:
                // 保持准备动画
                break;
                
            case PhantomPhase.Dashing:
                // 开始冲刺，但保持准备动画
                isDashing = true;
                break;
                
            case PhantomPhase.Attacking:
                // 停止冲刺，切换到攻击动画
                isDashing = false;
                player.anim.SetBool("PhantomPrepare", false);
                player.anim.SetBool("PhantomAttack", true);
                triggerCalled = false; // 重置动画触发器
                break;
                
            case PhantomPhase.Ending:
                // 停止所有动作
                isDashing = false;
                break;
        }
        
        Debug.Log($"幻影攻击阶段切换到: {newPhase}");
    }
    
    /// <summary>
    /// 设置冲刺速度
    /// </summary>
    public void SetDashSpeed(int speed)
    {
        dashSpeed = speed;
    }
    
    /// <summary>
    /// 结束幻影攻击，返回idle状态
    /// </summary>
    public void EndPhantomAttack()
    {
        stateMachine.ChangeState(player.idleState);
    }
}