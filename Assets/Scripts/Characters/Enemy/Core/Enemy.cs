using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{


    [Header("移动信息")]
    public float idleTime;

    [Header("玩家检测")]
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckDistance;
    [SerializeField] protected LayerMask WhatIsPlayer;
    protected RaycastHit2D isPlayerDetected;
    protected bool isGrounded;
    protected bool isWall;
    [Header("攻击信息")]
    public float AttackCoolDown;
    public float AttackCoolDownTimer;

    [Header("受击信息")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;
    //状态机
    public EnemyStateMachine stateMachine{get;private set;}
    public string lastAnimBoolName {get;private set;}




    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        AttackCoolDownTimer -= Time.deltaTime;
    }

    public override void SlowEntityBy(int _slowPercentage,float _slowDuration)
    {
        Stats.moveSpeedPercentage.AddModifier(-_slowPercentage);
        anim.speed = moveSpeed;
        StartCoroutine(Stats.moveSpeedPercentage.RemoveModifierCoroutine(-_slowPercentage,_slowDuration));
    }

    public virtual void FreezeTimer(bool _timeFrozen)
    {
        if(_timeFrozen)
        {
            Stats.moveSpeedPercentage.AddModifier(-Stats.moveSpeedPercentage.GetValue());
            anim.speed = 0;
        }
        else
        {
            Stats.moveSpeedPercentage.AddModifier(Stats.moveSpeedPercentage.GetValue());
            anim.speed = 1;
        }
    }
    
    protected virtual IEnumerator FreezeTimeFor(float _seconds)
    {
        FreezeTimer(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTimer(false);
    }
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }
    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    public virtual bool CanBeStunned()
    {
        if(canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    public virtual void AssignLastAnimAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }
    public RaycastHit2D PlayerDetected()=>
        Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, WhatIsPlayer);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, playerCheck.position + Vector3.right * playerCheckDistance * (isRightFacing ? 1 : -1));
    }

}
