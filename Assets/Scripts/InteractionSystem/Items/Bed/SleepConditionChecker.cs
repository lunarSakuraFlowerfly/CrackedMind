using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 睡眠条件检查器
/// </summary>
public class SleepConditionChecker : MonoBehaviour
{
    #region Singleton
    public static SleepConditionChecker Instance { get; private set; }
    
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

    private void Start()
    {
        InitializeSleepConditions();
    }

    /// <summary>
    /// 初始化所有睡眠体哦阿健
    /// </summary>
    private void InitializeSleepConditions()
    {
        // 找到场景中所有的ISleepCondition实现
        // 由于接口不能直接查找，我们需要查找已知的实现类
        KeyPressedSleepCondition[] keyConditions = FindObjectsOfType<KeyPressedSleepCondition>();
        foreach (var condition in keyConditions)
        {
            RegisterCondition(condition);
            Debug.Log($"<color=green>自动注册睡眠条件: {condition.GetType().Name}</color>");
        }
        // 如果没有找到任何条件，创建一个KeyPressedSleepCondition
        if (m_SleepConditions.Count == 0)
        {
            Debug.LogWarning("<color=yellow>未找到任何睡眠条件，自动创建KeyPressedSleepCondition</color>");
            GameObject conditionObj = new GameObject("KeyPressedSleepCondition");
            conditionObj.transform.SetParent(transform);
            KeyPressedSleepCondition newCondition = conditionObj.AddComponent<KeyPressedSleepCondition>();
            // 不需要手动注册，因为AddComponent会触发OnEnable
        }
    
        Debug.Log($"<color=cyan>睡眠条件检查器已初始化，共找到 {m_SleepConditions.Count} 个条件</color>");
    }
    #endregion

    #region Sleep Conditions
    // 所有睡眠条件检查器
    private List<ISleepCondition> m_SleepConditions = new List<ISleepCondition>();
    
    // 默认睡眠条件
    [Tooltip("如果没有注册任何条件，使用此默认值")]
    [SerializeField] private bool m_DefaultCanSleep = false;
    #endregion
    
    #region Public Methods
    /// <summary>
    /// 注册睡眠条件检查器
    /// </summary>
    public void RegisterCondition(ISleepCondition _condition)
    {
        if (_condition != null && !m_SleepConditions.Contains(_condition))
        {
            m_SleepConditions.Add(_condition);
        }
    }
    
    /// <summary>
    /// 注销睡眠条件检查器
    /// </summary>
    public void UnregisterCondition(ISleepCondition _condition)
    {
        if (_condition != null && m_SleepConditions.Contains(_condition))
        {
            m_SleepConditions.Remove(_condition);
        }
    }
    
    /// <summary>
    /// 检查是否满足所有睡眠条件
    /// </summary>
    public bool CanSleep()
    {
        if (m_SleepConditions == null || m_SleepConditions.Count == 0)
        {
            Debug.Log($"[SleepConditionChecker] 没有注册睡眠条件，使用默认值：{m_DefaultCanSleep}");
            return m_DefaultCanSleep;
        }

        Debug.Log($"[SleepConditionChecker] 检查 {m_SleepConditions.Count} 个睡眠条件");
        
        foreach (var condition in m_SleepConditions)
        {
            bool conditionResult = condition.CanSleep();
            Debug.Log($"[SleepConditionChecker] 条件 {condition.GetType().Name} 结果: {conditionResult}");
            
            if (!conditionResult)
            {
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// 获取无法睡觉的原因
    /// </summary>
    public string GetCannotSleepReason()
    {
        if (m_SleepConditions == null || m_SleepConditions.Count == 0)
        {
            return m_DefaultCanSleep ? "" : "无法入睡，没有注册任何睡眠条件";
        }

        foreach (var condition in m_SleepConditions)
        {
            if (!condition.CanSleep())
            {
                string reason = condition.GetCannotSleepReason();
                if (string.IsNullOrEmpty(reason))
                {
                    // 如果条件没有提供原因，使用通用信息
                    reason = $"无法入睡 ({condition.GetType().Name})";
                }
                return reason;
            }
        }

        return "";
    }
    
    /// <summary>
    /// 设置默认睡眠条件
    /// </summary>
    public void SetDefaultCanSleep(bool _canSleep)
    {
        m_DefaultCanSleep = _canSleep;
    }
    #endregion
}

/// <summary>
/// 睡眠条件接口
/// </summary>
public interface ISleepCondition
{
    bool CanSleep();
    string GetCannotSleepReason();
}
