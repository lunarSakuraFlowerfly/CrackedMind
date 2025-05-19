using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Player : Entity
{

    //动作延迟
    public bool isBusy{get;private set;}


    [Header("攻击信息")]
    public Vector2[] attackMovement;



    [Header("冲刺信息")]
    [SerializeField] public float dashDuration;
    [SerializeField] public float dashSpeed;
    public float dashCoolDown;
   
    
    #region 状态
    public PlayerStateMachine stateMachine {get;private set;}
    public PlayerIdleState idleState {get;private set;}
    public PlayerMoveState moveState {get;private set;}
    public PlayerJumpState jumpState {get;private set;}
    public PlayerAirState airState {get;private set;}
    public PlayerDashState dashState {get;private set;}
    public PlayerWallSlideState wallSlideState {get;private set;}
    public PlayerWallJump wallJump {get;private set;}
    public PlayerPrimaryAttack primaryAttack {get;private set;}
    #endregion

    #region 周期函数
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this,stateMachine,"Move");
        jumpState = new PlayerJumpState(this,stateMachine,"Jump");
        airState = new PlayerAirState(this,stateMachine,"Jump");
        dashState = new PlayerDashState(this,stateMachine,"Dash");
        wallSlideState = new PlayerWallSlideState(this,stateMachine,"WallSlide");
        wallJump = new PlayerWallJump(this,stateMachine,"Jump");
        primaryAttack = new PlayerPrimaryAttack(this,stateMachine,"Attack");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        base.Update();
        
        stateMachine.currentState.Update();
        

    }
    #endregion


    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

}
