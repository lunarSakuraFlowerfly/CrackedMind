using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 相机控制器，实现平滑跟随目标的功能
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("跟随设置")]
    [Tooltip("要跟随的目标")]
    [SerializeField] private Transform target;
    
    [Tooltip("相机跟随速度")]
    [SerializeField] private float followSpeed = 5f;
    
    [Tooltip("相机与目标的偏移量")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    
    [Tooltip("是否启用平滑跟随")]
    [SerializeField] private bool useSmoothFollow = true;
    
    [Tooltip("平滑跟随的插值速度")]
    [SerializeField] private float smoothTime = 0.3f;

    [Header("边界设置")]
    [Tooltip("是否启用边界限制")]
    [SerializeField] private bool useBounds = false;
    
    [Tooltip("相机移动的最小X坐标")]
    [SerializeField] private float minX = -10f;
    
    [Tooltip("相机移动的最大X坐标")]
    [SerializeField] private float maxX = 10f;
    
    [Tooltip("相机移动的最小Y坐标")]
    [SerializeField] private float minY = -10f;
    
    [Tooltip("相机移动的最大Y坐标")]
    [SerializeField] private float maxY = 10f;

    [Header("子物体设置")]
    [Tooltip("相机是否作为目标的子物体")]
    [SerializeField] private bool isChildOfTarget = false;

    // 私有变量
    private Vector3 currentVelocity;
    private Camera mainCamera;
    private bool m_ForceUpdate = false;
    private bool m_WasChildOfTarget = false;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        
        // 检查相机是否已经是目标的子物体
        if (target != null)
        {
            m_WasChildOfTarget = transform.parent == target;
        }
    }

    private void Start()
    {
        // 如果没有设置目标，尝试找到带有PlayerMovement组件的物体
        if (target == null)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                target = player.transform;
                Debug.Log("自动找到玩家目标：" + target.name);
            }
            else
            {
                Debug.LogWarning("未找到玩家目标，请在Inspector中手动设置！");
                enabled = false;
                return;
            }
        }
        
        // 如果设置为子物体模式，将相机设置为目标的子物体
        if (isChildOfTarget && target != null && transform.parent != target)
        {
            transform.SetParent(target);
            m_WasChildOfTarget = true;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        // 如果相机是目标的子物体，不需要手动更新位置
        if (transform.parent == target)
        {
            return;
        }

        // 计算目标位置
        Vector3 targetPosition = target.position + offset;

        // 应用边界限制
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // 更新相机位置
        if (useSmoothFollow && !m_ForceUpdate)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                smoothTime
            );
        }
        else
        {
            transform.position = targetPosition;
            m_ForceUpdate = false;
        }
    }

    /// <summary>
    /// 设置新的跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        // 如果设置为子物体模式，将相机设置为目标的子物体
        if (isChildOfTarget && target != null && transform.parent != target)
        {
            transform.SetParent(target);
            m_WasChildOfTarget = true;
        }
    }

    /// <summary>
    /// 设置相机边界
    /// </summary>
    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBounds = true;
    }
    
    /// <summary>
    /// 清除相机边界限制
    /// </summary>
    public void ClearBounds()
    {
        useBounds = false;
    }
    
    /// <summary>
    /// 立即更新相机位置到目标位置
    /// </summary>
    public void UpdatePosition()
    {
        if (target == null) return;
        
        // 如果相机是目标的子物体，不需要手动更新位置
        if (transform.parent == target)
        {
            return;
        }
        
        // 计算目标位置
        Vector3 targetPosition = target.position + offset;
        
        // 应用边界限制
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
        
        // 立即设置相机位置
        transform.position = targetPosition;
    }
    
    /// <summary>
    /// 强制下一帧立即更新相机位置
    /// </summary>
    public void ForceUpdate()
    {
        m_ForceUpdate = true;
    }
    
    /// <summary>
    /// 设置相机偏移量
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    /// <summary>
    /// 获取当前目标
    /// </summary>
    public Transform GetTarget()
    {
        return target;
    }
    
    /// <summary>
    /// 设置相机是否作为目标的子物体
    /// </summary>
    public void SetAsChildOfTarget(bool asChild)
    {
        isChildOfTarget = asChild;
        
        if (isChildOfTarget && target != null && transform.parent != target)
        {
            transform.SetParent(target);
            m_WasChildOfTarget = true;
        }
        else if (!isChildOfTarget && transform.parent == target)
        {
            transform.SetParent(null);
            m_WasChildOfTarget = false;
        }
    }
} 