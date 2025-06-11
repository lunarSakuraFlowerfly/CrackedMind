using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 玩家控制管理器，用于管理玩家移动控制的启用和禁用
/// </summary>
public class PlayerControlManager : MonoBehaviour
{
    #region Singleton
    public static PlayerControlManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    [Header("玩家组件引用")]
    [SerializeField] private MonoBehaviour playerMovementScript; // 引用玩家移动控制脚本
    [SerializeField] private MonoBehaviour[] additionalControlScripts; // 其他需要禁用的控制脚本
    
    [Header("设置")]
    [SerializeField] private bool disablePhysics = true; // 是否在禁用控制时同时禁用物理
    
    // 事件
    public event Action OnControlDisabled;
    public event Action OnControlEnabled;
    
    // 状态变量
    private bool isControlEnabled = true;
    private Rigidbody2D playerRigidbody;
    private Vector2 savedVelocity;
    private float savedGravityScale;
    
    private void Start()
    {
        // 如果没有指定玩家移动脚本，尝试自动查找
        if (playerMovementScript == null)
        {
            // 尝试查找常见的玩家控制脚本名称
            playerMovementScript = GetComponent<MonoBehaviour>();
            
            if (playerMovementScript == null)
            {
                Debug.LogWarning("没有找到玩家移动脚本，请手动指定");
            }
        }
        
        // 获取刚体组件
        playerRigidbody = GetComponent<Rigidbody2D>();
    }
    
    /// <summary>
    /// 启用玩家控制
    /// </summary>
    public void EnableControl()
    {
        // 添加日志便于调试
        Debug.Log("PlayerControlManager: 正在尝试启用玩家控制");
        
        if (isControlEnabled)
        {
            Debug.Log("PlayerControlManager: 玩家控制已经启用，无需重复操作");
            return;
        }
        
        // 启用主移动脚本
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
            Debug.Log($"PlayerControlManager: 已启用主移动脚本 {playerMovementScript.GetType().Name}");
            
            // 尝试获取玩家移动组件并设置可移动状态
            if (playerMovementScript is PlayerMovement playerMovement)
            {
                playerMovement.SetMovementEnabled(true);
                Debug.Log("PlayerControlManager: 已调用PlayerMovement.SetMovementEnabled(true)");
            }
        }
        else
        {
            Debug.LogWarning("PlayerControlManager: 未找到主移动脚本，无法启用");
        }
        
        // 启用其他控制脚本
        foreach (var script in additionalControlScripts)
        {
            if (script != null)
            {
                script.enabled = true;
                Debug.Log($"PlayerControlManager: 已启用额外控制脚本 {script.GetType().Name}");
            }
        }
        
        // 恢复物理
        if (disablePhysics && playerRigidbody != null)
        {
            playerRigidbody.velocity = savedVelocity;
            playerRigidbody.gravityScale = savedGravityScale;
            playerRigidbody.constraints = RigidbodyConstraints2D.None;
            
            // 如果使用的是2D平台游戏，可能需要设置FreezeRotation
            playerRigidbody.freezeRotation = true;
            
            Debug.Log("PlayerControlManager: 已恢复物理系统");
        }
        
        isControlEnabled = true;
        
        // 确保时间缩放正常
        Time.timeScale = 1f;
        
        // 触发事件
        OnControlEnabled?.Invoke();
        
        Debug.Log("PlayerControlManager: 玩家控制已成功启用");
    }
    
    /// <summary>
    /// 禁用玩家控制
    /// </summary>
    public void DisableControl()
    {
        if (!isControlEnabled) return;
        
        Debug.Log("PlayerControlManager: 正在禁用玩家控制");
        
        // 保存当前移动数据用于设置idle状态
        Vector2 lastMoveDirection = Vector2.zero;
        
        // 禁用主移动脚本前，获取当前移动方向
        if (playerMovementScript != null && playerMovementScript is MonoBehaviour)
        {
            var playerMovement = playerMovementScript as MonoBehaviour;
            if (playerMovement.TryGetComponent<PlayerMovement>(out var movement))
            {
                lastMoveDirection = movement.GetMoveDirection();
                // 确保角色变为idle状态但保持方向
                if (movement.enabled)
                {
                    Debug.Log($"PlayerControlManager: 保存角色最后移动方向 {lastMoveDirection}");
                    // 先设置移动状态为禁用
                    movement.SetMovementEnabled(false);
                }
            }
        }
        
        // 禁用主移动脚本
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
            Debug.Log($"PlayerControlManager: 已禁用主移动脚本 {playerMovementScript.GetType().Name}");
        }
        
        // 禁用其他控制脚本
        foreach (var script in additionalControlScripts)
        {
            if (script != null)
            {
                script.enabled = false;
                Debug.Log($"PlayerControlManager: 已禁用额外控制脚本 {script.GetType().Name}");
            }
        }
        
        // 禁用物理
        if (disablePhysics && playerRigidbody != null)
        {
            // 保存当前速度和重力比例
            savedVelocity = playerRigidbody.velocity;
            savedGravityScale = playerRigidbody.gravityScale;
            
            // 停止移动并冻结位置
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.gravityScale = 0;
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            Debug.Log("PlayerControlManager: 已禁用物理系统");
        }
        
        // 设置角色为对应方向的idle状态
        ApplyIdleAnimationWithDirection(lastMoveDirection);
        
        isControlEnabled = false;
        
        // 触发事件
        OnControlDisabled?.Invoke();
        
        Debug.Log("PlayerControlManager: 玩家控制已成功禁用");
    }
    
    /// <summary>
    /// 应用与移动方向对应的idle动画状态
    /// </summary>
    private void ApplyIdleAnimationWithDirection(Vector2 direction)
    {
        // 获取角色动画控制器
        Animator animator = GetComponent<Animator>();
        if (animator == null) return;
        
        // 如果方向不为零，则设置动画方向但将isMoving设为false
        if (direction.magnitude > 0.1f)
        {
            // 根据最后的移动方向计算朝向角度
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            
            // 确保角度在0-360度范围内
            angle = (angle + 360f) % 360f;
            
            // 转换为动画系统使用的方向值（0-5，与PlayerMovement中的映射一致）
            float animDirection = MapAngleToDirection(angle);
            
            // 应用动画参数
            animator.SetFloat("Direction", animDirection);
            animator.SetBool("IsMoving", false);
            
            Debug.Log($"PlayerControlManager: 设置角色朝向方向 {animDirection}，进入idle状态");
        }
    }
    
    /// <summary>
    /// 将角度映射到六个方向（与PlayerMovement中方法一致）
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
    /// 获取玩家控制状态
    /// </summary>
    public bool IsControlEnabled()
    {
        return isControlEnabled;
    }
} 