using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家移动控制器，处理WASD键盘输入和角色移动
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动速度")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Tooltip("是否启用平滑移动")]
    [SerializeField] private bool useSmoothMovement = true;
    
    [Tooltip("平滑移动的插值速度")]
    [SerializeField] private float smoothTime = 0.1f;

    [Header("动画设置")]
    [Tooltip("动画控制器")]
    [SerializeField] private Animator animator;
    
    [Tooltip("移动阈值，小于此值视为静止")]
    [SerializeField] private float moveThreshold = 0.1f;

    [Header("调试设置")]
    [Tooltip("是否显示移动方向指示器")]
    [SerializeField] private bool showDebugGizmos = true;
    
    [Header("交互设置")]
    [Tooltip("是否允许移动")]
    [SerializeField] private bool canMove = true;

    [Header("状态设置")]
    [Tooltip("是否处于睡眠状态")]
    private bool isSleeping = false;

    // 私有变量
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private float currentAngle; // 当前移动角度
    private float lastValidDirection = 0f;
    private Vector2 lastMoveInput;
    private float inputSmoothTime = 0.1f; // 输入平滑时间
    private Vector2 inputVelocity;
    private float directionSmoothTime = 0.1f; // 方向平滑时间
    private float currentDirection;
    private float directionVelocity;


    // 添加事件委托，用于通知移动状态变化
    public delegate void MovementStateChangedHandler(bool canMove);
    public event MovementStateChangedHandler OnMovementStateChanged;

    //玩家移动到特定位置事件
    public delegate void PlayerMoveToRightPositionHandler();
    public event PlayerMoveToRightPositionHandler OnPlayerMoveToRightPosition;
    


    private void Awake()
    {
        // 获取或添加Rigidbody2D组件
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // 配置Rigidbody2D
        rb.gravityScale = 0f; // 禁用重力
        rb.freezeRotation = true; // 冻结旋转
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 连续碰撞检测
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 启用插值
        
        // 如果没有指定动画控制器，尝试获取
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // 只有在允许移动时才处理输入
        if (canMove)
        {
            // 获取输入
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            // 直接使用原始输入，不进行平滑处理
            moveInput = new Vector2(horizontal, vertical);
            
            // 如果输入大于阈值，更新最后有效输入
            if (moveInput.magnitude > moveThreshold)
            {
                lastMoveInput = moveInput.normalized;
            }
        }
        else
        {
            // 如果不允许移动，将输入设为零
            moveInput = Vector2.zero;
            // 确保速度也为零
            rb.velocity = Vector2.zero;
        }
        
        // 更新动画状态
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        // 只有在允许移动时才应用移动
        if (canMove)
        {
            // 计算移动向量
            Vector2 movement = moveInput.normalized * moveSpeed;
            
            // 应用移动
            if (useSmoothMovement)
            {
                // 使用平滑移动，但确保在没有输入时速度为零
                if (moveInput.magnitude < 0.01f)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    rb.velocity = Vector2.SmoothDamp(rb.velocity, movement, ref currentVelocity, smoothTime);
                }
            }
            else
            {
                // 直接设置速度
                rb.velocity = movement;
            }
            
            // 计算当前角度
            if (movement.magnitude > moveThreshold)
            {
                currentAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
                
                float targetDirection = MapAngleToDirection(currentAngle);
                
                // 平滑过渡方向
                currentDirection = Mathf.SmoothDampAngle(currentDirection, targetDirection, ref directionVelocity, directionSmoothTime);
                
                // 更新最后有效方向
                lastValidDirection = currentDirection;
            }
        }
        else
        {
            // 如果不允许移动，停止角色
            rb.velocity = Vector2.zero;
        }
    }
    
    /// <summary>
    /// 启用或禁用玩家移动
    /// </summary>
    /// <param name="enable">是否启用移动</param>
    public void SetMovementEnabled(bool enable)
    {
        if (canMove != enable)
        {
            canMove = enable;
            
            // 无论是启用还是禁用，都立即重置所有运动状态
            rb.velocity = Vector2.zero;
            moveInput = Vector2.zero;
            currentVelocity = Vector2.zero;
            inputVelocity = Vector2.zero;
            
            // 更新动画状态
            UpdateAnimationState();
            
            // 触发事件
            OnMovementStateChanged?.Invoke(canMove);
            
            Debug.Log($"玩家移动已{(canMove ? "启用" : "禁用")}");
            
            // 如果启用移动，等待玩家松开所有按键
            if (enable)
            {
                StartCoroutine(WaitForKeyRelease());
            }
        }
    }
    
    /// <summary>
    /// 等待玩家松开所有移动按键
    /// </summary>
    private IEnumerator WaitForKeyRelease()
    {
        // 等待玩家松开所有按键
        while (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f)
        {
            // 在等待期间保持角色静止
            rb.velocity = Vector2.zero;
            moveInput = Vector2.zero;
            yield return null;
        }
        
        Debug.Log("玩家已松开所有按键，恢复正常输入处理");
    }
    
    /// <summary>
    /// 获取当前移动状态
    /// </summary>
    /// <returns>是否允许移动</returns>
    public bool CanMove()
    {
        return canMove;
    }

    /// <summary>
    /// 更新动画状态
    /// </summary>
    private void UpdateAnimationState()
    {
           if (animator == null) return;

    if (moveInput.magnitude > moveThreshold)
    {
        // 使用atan2计算角度
        currentAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        
        // 将角度映射到六个方向
        float direction = MapAngleToDirection(currentAngle);
        
        // 只有当输入足够大时才更新最后的有效方向
        lastValidDirection = direction;
        
        // 设置动画参数
        animator.SetFloat("Direction", direction);
        animator.SetBool("IsMoving", true);
    }
    else
    {
        // 停止时保持最后的有效方向
        animator.SetFloat("Direction", lastValidDirection);
        animator.SetBool("IsMoving", false);
    }
    }

    /// <summary>
    /// 将角度映射到六个方向
    /// </summary>
    private float MapAngleToDirection(float angle)
    {
        // 确保角度在0-360范围内
        angle = (angle + 360f) % 360f;

        // 定义六个方向的角度范围（每个方向60度）
        // 0: 前 (向上, -30 到 30)
        // 1: 右前 (30 到 90)
        // 2: 右后 (90 到 150)
        // 3: 后 (150 到 210)
        // 4: 左后 (210 到 270)
        // 5: 左前 (270 到 330)

        if (angle >= -30f && angle < 30f) return 0f;       // 前
        if (angle >= 30f && angle < 90f) return 5f;        // 右前
        if (angle >= 90f && angle < 150f) return 4f;       // 右后
        if (angle >= 150f && angle < 210f) return 3f;      // 后
        if (angle >= 210f && angle < 270f) return 2f;      // 左后
        if (angle >= 270f && angle < 330f) return 1f;      // 左前
        return 0f;                                         // 默认前
    }

    /// <summary>
    /// 在编辑器中可视化移动方向
    /// </summary>
    private void OnDrawGizmos()
    {
        if (showDebugGizmos && Application.isPlaying)
        {
            // 绘制移动方向指示器
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, moveInput);
            
            // 绘制当前速度指示器
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, rb.velocity.normalized);
        }
    }

    /// <summary>
    /// 获取当前移动方向
    /// </summary>
    public Vector2 GetMoveDirection()
    {
        return moveInput;
    }

    /// <summary>
    /// 获取最后有效的移动方向值
    /// </summary>
    /// <returns>最后有效的方向值（0-5之间的值）</returns>
    public float GetLastValidDirection()
    {
        return lastValidDirection;
    }

    /// <summary>
    /// 获取当前移动速度
    /// </summary>
    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }

    /// <summary>
    /// 获取当前移动角度
    /// </summary>
    public float GetCurrentAngle()
    {
        return currentAngle;
    }

    //移动到特定位置
    public void MoveTo(Vector3 targetPosition,float duration)
    {
        StartCoroutine(MoveToCoroutine(targetPosition,duration));
    }

    private IEnumerator MoveToCoroutine(Vector3 targetPosition,float duration)
    {
        
        rb.velocity = Vector2.zero;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        
        // 提前计算方向，确保动画立即响应
        Vector2 moveDirection = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
        
  
        lastValidDirection = MapAngleToDirection(angle);
        while(elapsedTime < duration)
        {
            canMove = false;
            animator.SetBool("IsMoving", true);
            animator.SetFloat("Direction", MapAngleToDirection(angle));
            // 每帧更新位置
            float t = elapsedTime / duration;
            // 可以使用平滑曲线改善移动感
            t = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            
            // 更新已用时间
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保精确到达目标位置
        transform.position = targetPosition;
        
        // 停止移动动画
        animator.SetBool("IsMoving", false);
        canMove = true;

        // 触发事件
        OnPlayerMoveToRightPosition?.Invoke();
    }
    
    /// <summary>
    /// 设置睡眠状态
    /// </summary>
    /// <param name="sleeping">是否进入睡眠状态</param>
    public void SetSleepState(bool sleeping)
    {
        if (isSleeping != sleeping)
        {
            isSleeping = sleeping;
            canMove = !sleeping;
            
            if (animator != null)
            {
                animator.SetBool("IsSleeping", sleeping);
                
                // 如果进入睡眠状态，确保角色停止移动
                if (sleeping)
                {
                    rb.velocity = Vector2.zero;
                    moveInput = Vector2.zero;
                    animator.SetBool("IsMoving", false);
                }
            }
            
            Debug.Log($"玩家睡眠状态已{(sleeping ? "开启" : "关闭")}");
        }
    }

    /// <summary>
    /// 获取当前睡眠状态
    /// </summary>
    /// <returns>是否处于睡眠状态</returns>
    public bool IsSleeping()
    {
        return isSleeping;
    }

    /// <summary>
    /// 设置目前的动画朝向位置
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    public void SetLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        lastValidDirection = MapAngleToDirection(angle);
        animator.SetFloat("Direction", lastValidDirection);
    }
}
