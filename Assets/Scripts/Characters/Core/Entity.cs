using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Rigidbody2D rb{get;private set;}
    public Animator anim{get;private set;}
    public EntityFX fx{get;private set;}
    public SpriteRenderer sr{get;private set;}

    public CharacterStats Stats{get;private set;}

    public CapsuleCollider2D cd{get;private set;}

    #region 方向
    public bool isRightFacing{get;private set;} = true;
    public float facingDirection{get;private set;} = 1;

    public System.Action onFlipped;
    #endregion

    [Header("移动信息")]
    [SerializeField]public float moveSpeed;
    [SerializeField]public float jumpForce;
    protected float defaultMoveSpeed;
    protected float defaultJumpForce;

    [Header("碰撞信息")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask WhatIsGround;
    [SerializeField] protected Transform groundCheck;
    [Space]
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected Transform wallCheck;

    [Header("受击信息")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;


    #region 周期函数
    protected virtual void Awake()
    {
        facingDirection = 1;
        isRightFacing = true;
    }
    protected virtual void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        fx = GetComponentInChildren<EntityFX>();
        sr = GetComponentInChildren<SpriteRenderer>();
        Stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }
    protected virtual void Update()
    {
        
    }
    #endregion

    #region 碰撞检测
    public bool isGroundedDetected()=>
        Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, WhatIsGround);
    public bool isWallDetected()=>
        Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, WhatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance*(isRightFacing ? 1 : -1), wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region 运动设置
    public virtual void Flip()
    {
        isRightFacing = !isRightFacing;
        transform.Rotate(0, 180, 0);
        facingDirection = -facingDirection;
        onFlipped?.Invoke();
    }
    public void FilpController(float _xVelocity)
    {
        if(_xVelocity > 0 && !isRightFacing)
        {
            Flip();
        }
        else if(_xVelocity < 0 && isRightFacing)
        {   
            Flip();
        }
    }

    public virtual void SetVelocity(float _xVelocity,float _yVelocity)
    {
        if(isKnocked) return;
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FilpController(_xVelocity);
    }
    public void ZeroVelocity()
    {
        if(isKnocked) return;
        rb.velocity = new Vector2(0,0);
    }

    public virtual void SlowEntityBy(float _slowPercentage,float _slowDuration)
    {

    }
    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }
    #endregion

    #region 战斗系统
    public virtual void DamageFX()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine(HitKnockback());
    }

    protected virtual IEnumerator HitKnockback()
    {
        
        isKnocked = true;
        rb.velocity = new Vector2(knockbackDirection.x * -facingDirection, knockbackDirection.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }
    #endregion

    public void MakeTransparent(bool _transparent)
    {
        if(_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    public virtual void Die()
    {
        
    }

    public void SelfDestroy()=>Destroy(gameObject);
}