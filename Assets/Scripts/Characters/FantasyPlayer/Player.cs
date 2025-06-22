using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{

    //动作延迟
    public bool isBusy{get;private set;}
    public GameObject Sword{get;private set;}


    [Header("攻击信息")]
    public Vector2[] attackMovement;
    public float counterAttackDuration;



    [Header("冲刺信息")]
    [SerializeField] public float dashDuration;    
    public float dashCoolDown;

    public float dashDir{get;private set;}

    [Header("剑信息")]
    public float swordReturnImpact;


    public SkillManager skill{get;private set;}
    
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
    public PlayerCounterAttackState counterAttackState {get;private set;}
    public PlayerAimSwordState aimSwordState {get;private set;}
    public PlayerCatchSwordState catchSwordState {get;private set;}
    public PlayerBlackholeState blackholeState {get;private set;}
    public PlayerDeadState deadState {get;private set;}
    public PlayerShieldState shieldState {get;private set;}
    public PlayerHeartSlashState heartSlashState {get;private set;}
    public PlayerDawnbreakerState dawnbreakerState {get;private set;}
    public PlayerHealState healState {get;private set;}
    public PlayerPhantomAttack phantomState {get;private set;}
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
        counterAttackState = new PlayerCounterAttackState(this,stateMachine,"CounterAttack");
        aimSwordState = new PlayerAimSwordState(this,stateMachine,"AimSword");
        catchSwordState = new PlayerCatchSwordState(this,stateMachine,"CatchSword");
        blackholeState = new PlayerBlackholeState(this,stateMachine,"Jump");
        deadState = new PlayerDeadState(this,stateMachine,"Dead");
        shieldState = new PlayerShieldState(this,stateMachine,"Shield");
        heartSlashState = new PlayerHeartSlashState(this,stateMachine,"Shield");
        dawnbreakerState = new PlayerDawnbreakerState(this,stateMachine,"Dawnbreaker");
        healState = new PlayerHealState(this,stateMachine,"Heal");
        phantomState = new PlayerPhantomAttack(this,stateMachine,"PhantomPrepare");
    }

    protected override void Start()
    {
        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        if(Time.timeScale==0) return;
        base.Update();
        
        stateMachine.currentState.Update();

    }
    #endregion

    public override void SlowEntityBy(int _slowPercentage,float _slowDuration)
    {
        base.SlowEntityBy(_slowPercentage,_slowDuration);
        //计算减速大小
        Stats.moveSpeedPercentage.AddModifier(-_slowPercentage);
        Stats.jumpForcePercentage.AddModifier(-_slowPercentage);
        Stats.dashSpeedPercentage.AddModifier(-_slowPercentage);

        StartCoroutine(Stats.moveSpeedPercentage.RemoveModifierCoroutine(-_slowPercentage,_slowDuration));
        StartCoroutine(Stats.jumpForcePercentage.RemoveModifierCoroutine(-_slowPercentage,_slowDuration));
        StartCoroutine(Stats.dashSpeedPercentage.RemoveModifierCoroutine(-_slowPercentage,_slowDuration));
    }


    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AssignNewSword(GameObject _newSword)=>Sword = _newSword;
    public void ClearTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(Sword);
    }
    public void ExitBlackholeAbility()
    {
        stateMachine.ChangeState(airState);
    }
    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
        
    }
    protected override void SetupZeroKnockbackPower()
    {
        base.SetupZeroKnockbackPower();
        knockbackDirection = Vector2.zero;
    }
}
