using UnityEngine;

/// <summary>
/// 基于任务的睡眠条件
/// </summary>
public class QuestSleepCondition : MonoBehaviour, ISleepCondition
{
    [SerializeField] private bool m_RequireMainQuestCompletion = true;
    [SerializeField] private string m_CannotSleepReason = "我还有重要的任务没有完成，现在不能睡觉。";
    
    private void OnEnable()
    {
        // 注册到条件检查器
        if (SleepConditionChecker.Instance != null)
        {
            SleepConditionChecker.Instance.RegisterCondition(this);
        }
    }
    
    private void OnDisable()
    {
        // 从条件检查器注销
        if (SleepConditionChecker.Instance != null)
        {
            SleepConditionChecker.Instance.UnregisterCondition(this);
        }
    }
    
    /// <summary>
    /// 检查是否可以睡觉
    /// </summary>
    public bool CanSleep()
    {
        // 这里可以检查任务系统中的任务完成情况
        // 示例代码:
        // if (m_RequireMainQuestCompletion && !QuestManager.Instance.IsMainQuestCompleted())
        // {
        //     return false;
        // }
        
        // 临时返回值，根据实际需求修改
        return true;
    }
    
    /// <summary>
    /// 获取无法睡觉的原因
    /// </summary>
    public string GetCannotSleepReason()
    {
        return m_CannotSleepReason;
    }
}
