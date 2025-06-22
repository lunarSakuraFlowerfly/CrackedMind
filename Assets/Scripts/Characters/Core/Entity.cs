using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    protected float moveSpeed;
    protected float jumpForce;
    protected float dashSpeed;
    protected float attackSpeed;
    #endregion


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
    public int knockbackDir{get;private set;}


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
        CalculateSpeed();
    }
    #endregion

    private void CalculateSpeed()
    {
        moveSpeed = Stats.GetMoveSpeed();
        jumpForce = Stats.GetJumpForce();
        dashSpeed = Stats.GetDashSpeed();
        attackSpeed = Stats.GetAttackSpeed();
    }

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

    public virtual void SlowEntityBy(int _slowPercentage,float _slowDuration)
    {

    }

    #endregion

    #region 战斗系统
    public virtual void DamageFX()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine(HitKnockback());
    }
    public virtual void DamageFX(float _force,Vector2 _direction,float _duration)
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine(BeatBackBy(_force,_direction,_duration));
    }
    public void SetupKnockbackPower(Vector2 _knockbackpower)=>knockbackDirection = _knockbackpower;
    protected virtual IEnumerator HitKnockback()
    {
        
        isKnocked = true;
        rb.velocity = new Vector2(knockbackDirection.x * knockbackDir, knockbackDirection.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        SetupZeroKnockbackPower();
    }
    public virtual IEnumerator BeatBackBy(float _force,Vector2 _direction,float _duration)
    {
        isKnocked = true;
        rb.velocity = new Vector2(_direction.x * _force, _direction.y * _force);
        Debug.Log("击退敌人速度：" + rb.velocity);
        yield return new WaitForSeconds(_duration);
        isKnocked = false;
    }

    public virtual void SetKnockbackDir(Transform _damageDir)
    {
        if(_damageDir.position.x > transform.position.x)
        {
            knockbackDir = -1;
        }
        else if(_damageDir.position.x < transform.position.x)
        {
            knockbackDir = 1;
        }
    }
    #endregion

    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    public virtual void Die()
    {
        
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    public float GetMoveSpeed()=>moveSpeed;
    public float GetJumpForce()=>jumpForce;
    public float GetDashSpeed()=>dashSpeed;
    public float GetAttackSpeed()=>attackSpeed;
    public void SelfDestroy()=>Destroy(gameObject);
}