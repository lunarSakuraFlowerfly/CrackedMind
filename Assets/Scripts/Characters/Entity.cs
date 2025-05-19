using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Rigidbody2D rb{get;private set;}
    public Animator anim{get;private set;}

    #region 方向
    public bool isRightFacing{get;private set;} = true;
    public float facingDirection{get;private set;} = 1;
    #endregion

    [Header("移动信息")]
    [SerializeField]public float moveSpeed;
    [SerializeField]public float jumpForce;
    [Header("碰撞信息")]
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask WhatIsGround;
    [SerializeField] protected Transform groundCheck;
    [Space]
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected Transform wallCheck;

    #region 周期函数
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

    }
    protected virtual void Start()
    {
        facingDirection = 1;
        isRightFacing = true;
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
    }
    #endregion

    #region 运动设置
    public virtual void Flip()
    {
        isRightFacing = !isRightFacing;
        transform.Rotate(0, 180, 0);
        facingDirection = -facingDirection;
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
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FilpController(_xVelocity);
    }
    public void ZeroVelocity()=>rb.velocity = new Vector2(0,0);
    #endregion

}