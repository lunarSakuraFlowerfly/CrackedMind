using UnityEngine;
using System.Collections;
/// <summary>
/// 按键按下睡眠条件
/// </summary>
public class KeyPressedSleepCondition : MonoBehaviour, ISleepCondition
{
    [Header("按键设置")]
    [SerializeField] private KeyCode m_UnlockSleepKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode m_LockSleepKey = KeyCode.Alpha3;

    [Header("提示信息")]
    [SerializeField] private string m_CannotSleepReason = "睡眠被锁定，请按2键解锁睡眠权限。";

    // 睡眠权限状态
    private bool m_SleepPermissionGranted = false;
    private bool m_Registered = false;

    private void Start()
    {
        // 在Start中使用协程延迟注册，确保SleepConditionChecker已完成初始化
        StartCoroutine(RegisterWithDelay());
    }
    
    private IEnumerator RegisterWithDelay()
    {
        // 等待一帧，确保所有Awake和OnEnable已执行
        yield return null;
        
        // 尝试注册到条件检查器
        RegisterToChecker();
    }
    
    private void RegisterToChecker()
    {
        if (m_Registered) return;
        
        if (SleepConditionChecker.Instance != null)
        {
            SleepConditionChecker.Instance.RegisterCondition(this);
            m_Registered = true;
            Debug.Log("<color=cyan>按键睡眠条件已注册</color>");
        }
        else
        {
            Debug.LogError("<color=red>SleepConditionChecker实例不存在，无法注册睡眠条件</color>");
        }
    }

    private void OnDisable()
    {
        // 从条件检查器注销
        if (SleepConditionChecker.Instance != null && m_Registered)
        {
            SleepConditionChecker.Instance.UnregisterCondition(this);
            m_Registered = false;
        }
    }
    
    private void Update()
    {
        // 如果还未注册，尝试再次注册
        if (!m_Registered)
        {
            RegisterToChecker();
        }
        
        // 检测按键解锁/锁定睡眠权限
        if (Input.GetKeyDown(m_UnlockSleepKey))
        {
            m_SleepPermissionGranted = true;
            Debug.Log("<color=green>睡眠权限已解锁！按2键生效</color>");
        }
        else if (Input.GetKeyDown(m_LockSleepKey))
        {
            m_SleepPermissionGranted = false;
            Debug.Log("<color=orange>睡眠权限已锁定！按3键生效</color>");
        }
    }

    /// <summary>
    /// 检查是否可以睡觉
    /// </summary>
    public bool CanSleep()
    {
        // 简单的按键检测条件
        return m_SleepPermissionGranted;
    }
    
    /// <summary>
    /// 获取无法睡觉的原因
    /// </summary>
    public string GetCannotSleepReason()
    {
        return m_CannotSleepReason;
    }
}
