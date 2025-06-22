using System;
using UnityEngine;

/// <summary>
/// 梦境退出管理器，控制从梦境中醒来的条件和流程
/// </summary>
public class DreamExit : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool m_EnableTestKey = true;
    [SerializeField] private KeyCode m_TestExitKey = KeyCode.Alpha1;
    
    [Header("效果引用")]
    [SerializeField] private SleepWakeEffect m_SleepEffect;
    
    // 定义委托和事件，用于自定义醒来条件
    public delegate bool WakeConditionHandler();
    public event WakeConditionHandler OnCheckWakeCondition;
    
    // 醒来事件，可以用于通知其他系统玩家已经醒来
    public event Action OnWakeFromDream;
    
    private void Update()
    {
        // 测试按键检查
        if (m_EnableTestKey && Input.GetKeyDown(m_TestExitKey))
        {
            ExitDream();
            return;
        }
        
        // 检查自定义醒来条件
        if (CheckWakeCondition())
        {
            ExitDream();
        }
    }
    
    /// <summary>
    /// 检查是否满足醒来条件
    /// </summary>
    /// <returns>是否满足醒来条件</returns>
    private bool CheckWakeCondition()
    {
        // 如果有注册的条件检查器，则使用它们
        if (OnCheckWakeCondition != null)
        {
            // 使用委托检查所有条件
            return OnCheckWakeCondition.Invoke();
        }
        
        // 默认不满足条件
        return false;
    }
    
    /// <summary>
    /// 退出梦境，返回现实
    /// </summary>
    public void ExitDream()
    {
        if (m_SleepEffect != null)
        {
            m_SleepEffect.WakeUp();
            // 触发醒来事件
            OnWakeFromDream?.Invoke();
        }
        else
        {
            Debug.LogError("未设置SleepWakeEffect引用，无法退出梦境");
        }
    }
    
    /// <summary>
    /// 添加自定义醒来条件
    /// </summary>
    /// <param name="condition">条件检查函数</param>
    public void AddWakeCondition(WakeConditionHandler condition)
    {
        OnCheckWakeCondition += condition;
    }
    
    /// <summary>
    /// 移除自定义醒来条件
    /// </summary>
    /// <param name="condition">条件检查函数</param>
    public void RemoveWakeCondition(WakeConditionHandler condition)
    {
        OnCheckWakeCondition -= condition;
    }
}
