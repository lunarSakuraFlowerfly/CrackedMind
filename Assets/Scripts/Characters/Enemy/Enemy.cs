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

    //状态机
    public EnemyStateMachine stateMachine{get;private set;}

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
    public RaycastHit2D PlayerDetected()=>
        Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, WhatIsPlayer);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, playerCheck.position + Vector3.right * playerCheckDistance * (isRightFacing ? 1 : -1));
    }

}
