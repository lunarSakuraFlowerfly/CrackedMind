using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 梦境玩家控制器 - 处理横屏角色的所有行为
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class DreamPlayerController : MonoBehaviour
{
    #region 组件引用
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    #endregion

    #region 事件定义
    public event Action OnPlayerDeath;
    public event Action OnPlayerHit;
    public event Action OnPlayerJump;
    public event Action OnPlayerLand;
    public event Action OnPlayerAttack;
    public event Action OnPlayerDash;
    #endregion

    #region 序列化字段
    [Header("玩家状态")]
    [SerializeField] private PlayerStatus _playerStatus;
    [SerializeField] private bool _isDead = false;
    [SerializeField] private bool _isInvincible = false;

    [Header("移动参数")]
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _acceleration = 50f;
    [SerializeField] private float _deceleration = 50f;
    [SerializeField] private float _runSpeedMultiplier = 1.5f;
    [SerializeField] private float _airControlFactor = 0.5f;
    
    [Header("跳跃参数")]
    [SerializeField] private float _jumpForce = 15f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private int _maxJumpCount = 1;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private float _jumpBufferTime = 0.1f;
    
    [Header("冲刺参数")]
    [SerializeField] private float _dashForce = 25f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;
    
    [Header("攻击参数")]
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 0.5f;
    [SerializeField] private LayerMask _enemyLayers;
    
    [Header("地面检测")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayers;
    
    [Header("受击参数")]
    [SerializeField] private float _hitStunDuration = 0.3f;
    [SerializeField] private float _invincibilityDuration = 1f;
    [SerializeField] private Color _hitFlashColor = Color.red;
    [SerializeField] private float _knockbackForce = 5f;
    #endregion

    #region 私有字段
    private float _moveInput;
    private float _lastGroundedTime;
    private float _lastJumpTime;
    private int _jumpCount;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isDashing;
    private bool _isAttacking;
    private bool _isRunning;
    private bool _isHit;
    private bool _isFacingRight = true;
    private float _dashTimeLeft;
    private float _dashCooldownTimeLeft;
    private float _attackCooldownTimeLeft;
    private Vector2 _dashDirection;
    private Color _originalColor;
    #endregion

    #region Unity生命周期
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        
        _originalColor = _spriteRenderer.color;
        
        if (_attackPoint == null)
        {
            _attackPoint = transform;
        }
        
        if (_groundCheckPoint == null)
        {
            _groundCheckPoint = transform;
        }
    }

    private void Start()
    {
        InitializePlayerStats();
    }

    private void Update()
    {
        if (_isDead) return;
        
        HandleTimers();
        HandleInput();
        CheckGrounded();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        
        if (_isHit) return;
        
        if (_isDashing)
        {
            HandleDash();
        }
        else
        {
            HandleMovement();
            HandleJumpGravity();
        }
    }
    #endregion

    #region 初始化
    /// <summary>
    /// 初始化玩家状态
    /// </summary>
    private void InitializePlayerStats()
    {
        if (_playerStatus != null)
        {
            _maxSpeed = _playerStatus.MoveSpeed / 10f; // 调整为合适的控制值
        }
    }
    #endregion

    #region 输入处理
    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInput()
    {
        // 在实际游戏中，这里会使用Input System或其他输入系统
        // 这里使用传统输入系统作为示例
        _moveInput = Input.GetAxisRaw("Horizontal");
        
        // 奔跑控制
        _isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // 跳跃缓冲
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _lastJumpTime = Time.time;
        }
        
        // 冲刺输入
        if (Input.GetKeyDown(KeyCode.LeftControl) && _dashCooldownTimeLeft <= 0 && !_isDashing)
        {
            StartDash();
        }
        
        // 攻击输入
        if (Input.GetMouseButtonDown(0) && _attackCooldownTimeLeft <= 0 && !_isAttacking)
        {
            StartAttack();
        }
    }
    #endregion

    #region 计时器处理
    /// <summary>
    /// 处理所有计时器
    /// </summary>
    private void HandleTimers()
    {
        // 冲刺冷却
        if (_dashCooldownTimeLeft > 0)
        {
            _dashCooldownTimeLeft -= Time.deltaTime;
        }
        
        // 攻击冷却
        if (_attackCooldownTimeLeft > 0)
        {
            _attackCooldownTimeLeft -= Time.deltaTime;
        }
        
        // 土狼时间（Coyote Time）- 给予玩家在离开平台后短暂的跳跃窗口
        if (_isGrounded)
        {
            _lastGroundedTime = Time.time;
            _jumpCount = 0;
        }
        
        // 尝试跳跃（如果在缓冲时间内并且满足条件）
        if (Time.time - _lastJumpTime <= _jumpBufferTime && !_isJumping && (Time.time - _lastGroundedTime <= _coyoteTime || (_jumpCount < _maxJumpCount && _maxJumpCount > 1)))
        {
            Jump();
        }
    }
    #endregion

    #region 地面检测
    /// <summary>
    /// 检测玩家是否在地面上
    /// </summary>
    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayers);
        
        // 检测着陆
        if (_isGrounded && _rb.velocity.y <= 0.1f)
        {
            if (_isJumping)
            {
                _isJumping = false;
                OnPlayerLand?.Invoke();
            }
        }
    }
    #endregion

    #region 移动处理
    /// <summary>
    /// 处理玩家移动
    /// </summary>
    private void HandleMovement()
    {
        float targetSpeed = _moveInput * (_isRunning ? _maxSpeed * _runSpeedMultiplier : _maxSpeed);
        float speedDiff = targetSpeed - _rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _deceleration;
        
        // 在空中时减少控制力
        if (!_isGrounded)
        {
            accelRate *= _airControlFactor;
        }
        
        // 计算移动力
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, 0.96f) * Mathf.Sign(speedDiff);
        
        // 应用移动力
        _rb.AddForce(movement * Vector2.right);
        
        // 处理角色朝向
        if (_moveInput > 0 && !_isFacingRight)
        {
            Flip();
        }
        else if (_moveInput < 0 && _isFacingRight)
        {
            Flip();
        }
    }
    
    /// <summary>
    /// 翻转角色朝向
    /// </summary>
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    #endregion

    #region 跳跃处理
    /// <summary>
    /// 执行跳跃
    /// </summary>
    private void Jump()
    {
        _lastJumpTime = 0; // 重置跳跃缓冲
        _isJumping = true;
        _jumpCount++;
        
        // 设置垂直速度而不是直接施加力，使跳跃更精确
        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        
        OnPlayerJump?.Invoke();
    }
    
    /// <summary>
    /// 处理跳跃重力修改（使跳跃感觉更好）
    /// </summary>
    private void HandleJumpGravity()
    {
        // 下落时增加重力
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime);
        }
        // 短跳时减少向上速度
        else if (_rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }
    #endregion

    #region 冲刺处理
    /// <summary>
    /// 开始冲刺
    /// </summary>
    private void StartDash()
    {
        _isDashing = true;
        _dashTimeLeft = _dashDuration;
        _dashCooldownTimeLeft = _dashCooldown;
        
        // 确定冲刺方向
        if (Mathf.Abs(_moveInput) > 0.1f)
        {
            _dashDirection = new Vector2(_moveInput, 0).normalized;
        }
        else
        {
            _dashDirection = new Vector2(_isFacingRight ? 1 : -1, 0);
        }
        
        // 冲刺开始时停止当前速度
        _rb.velocity = Vector2.zero;
        
        // 冲刺时可以短暂无敌
        StartCoroutine(BecomeInvincible(_dashDuration));
        
        OnPlayerDash?.Invoke();
    }
    
    /// <summary>
    /// 处理冲刺过程
    /// </summary>
    private void HandleDash()
    {
        if (_dashTimeLeft > 0)
        {
            _dashTimeLeft -= Time.fixedDeltaTime;
            _rb.velocity = _dashDirection * _dashForce;
            
            // 冲刺结束
            if (_dashTimeLeft <= 0)
            {
                _isDashing = false;
                _rb.velocity = _dashDirection * _maxSpeed; // 保持一定动量
            }
        }
    }
    #endregion

    #region 攻击处理
    /// <summary>
    /// 开始攻击
    /// </summary>
    private void StartAttack()
    {
        _isAttacking = true;
        _attackCooldownTimeLeft = _attackCooldown;
        
        StartCoroutine(PerformAttack());
        
        OnPlayerAttack?.Invoke();
    }
    
    /// <summary>
    /// 执行攻击
    /// </summary>
    private IEnumerator PerformAttack()
    {
        // 检测攻击范围内敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _enemyLayers);
        
        // 对敌人造成伤害
        foreach (Collider2D enemy in hitEnemies)
        {
            // 这里应调用敌人的受伤方法
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float damage = 0;
                if (_playerStatus != null)
                {
                    damage = _playerStatus.CalculateDamage();
                }
                else
                {
                    damage = 10f; // 默认伤害
                }
                
                damageable.TakeDamage(damage);
            }
        }
        
        // 攻击动画时间
        yield return new WaitForSeconds(_attackCooldown * 0.8f);
        
        _isAttacking = false;
    }
    #endregion

    #region 受击和死亡处理
    /// <summary>
    /// 玩家受击
    /// </summary>
    public void TakeHit(float damage, Vector2 knockbackDirection)
    {
        if (_isInvincible || _isDead) return;
        
        if (_playerStatus != null)
        {
            // 计算减伤后的实际伤害
            float actualDamage = _playerStatus.CalculateDamageReduction(damage);
            
            // 增加惊觉值
            _playerStatus.AwakeValue += actualDamage;
        }
        
        _isHit = true;
        
        // 击退
        _rb.velocity = Vector2.zero;
        _rb.AddForce(knockbackDirection.normalized * _knockbackForce, ForceMode2D.Impulse);
        
        // 无敌时间
        StartCoroutine(BecomeInvincible(_invincibilityDuration));
        
        // 受击效果
        StartCoroutine(HitStun());
        StartCoroutine(HitFlash());
        
        OnPlayerHit?.Invoke();
    }
    
    /// <summary>
    /// 玩家无敌状态
    /// </summary>
    private IEnumerator BecomeInvincible(float duration)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }
    
    /// <summary>
    /// 受击硬直
    /// </summary>
    private IEnumerator HitStun()
    {
        yield return new WaitForSeconds(_hitStunDuration);
        _isHit = false;
    }
    
    /// <summary>
    /// 受击闪烁效果
    /// </summary>
    private IEnumerator HitFlash()
    {
        _spriteRenderer.color = _hitFlashColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = _originalColor;
    }
    
    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        _rb.velocity = Vector2.zero;
        
        // 禁用碰撞
        _collider.enabled = false;
        
        // 播放死亡动画
        _animator.SetTrigger("Die");
        
        OnPlayerDeath?.Invoke();
    }
    #endregion

    #region 动画控制
    /// <summary>
    /// 更新角色动画
    /// </summary>
    private void UpdateAnimator()
    {
        // 这里应根据项目中实际使用的动画参数进行修改
        _animator.SetFloat("Speed", Mathf.Abs(_moveInput));
        _animator.SetBool("IsRunning", _isRunning && Mathf.Abs(_moveInput) > 0.1f);
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetBool("IsJumping", _isJumping);
        _animator.SetBool("IsDashing", _isDashing);
        _animator.SetBool("IsAttacking", _isAttacking);
        _animator.SetBool("IsHit", _isHit);
    }
    #endregion

    #region 调试
    /// <summary>
    /// 在Scene视图中绘制调试辅助线
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_groundCheckPoint == null) 
            _groundCheckPoint = transform;
            
        if (_attackPoint == null)
            _attackPoint = transform;
        
        // 地面检测范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
        
        // 攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
    }
    #endregion
}

/// <summary>
/// 可伤害接口 - 用于敌人和其他可受伤害对象
/// </summary>
public interface IDamageable
{
    void TakeDamage(float damage);
}
