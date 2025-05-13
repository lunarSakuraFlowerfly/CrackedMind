using UnityEngine;
using System;

/// <summary>
/// 玩家状态管理器 - 使用ScriptableObject实现数据持久化
/// </summary>
[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Game/Player Status")]
public class PlayerStatus : ScriptableObject
{
    #region 事件定义
    public event Action<float> OnAwakeValueChanged;
    public event Action<float> OnMentalThresholdChanged;
    public event Action<int> OnSoulLevelChanged;
    public event Action<float> OnCourageChanged;
    public event Action<float> OnWillpowerChanged;
    public event Action<float> OnMoveSpeedChanged;
    public event Action<float> OnAttackSpeedChanged;
    public event Action<float> OnHopeChanged;
    #endregion

    #region 基础属性
    [Header("惊觉值")]
    [SerializeField] private float _awakeValue;
    [SerializeField] private float _maxAwakeValue = 100f;
    [SerializeField] private float _minAwakeValue = 0f;

    [Header("心理阈值")]
    [SerializeField] private float _mentalThreshold;
    [SerializeField] private float _maxMentalThreshold = 100f;
    [SerializeField] private float _minMentalThreshold = 0f;

    [Header("心灵等级")]
    [SerializeField] private int _soulLevel = 1;
    [SerializeField] private int _maxSoulLevel = 50;
    [SerializeField] private float _currentExp;
    [SerializeField] private float _expToNextLevel = 100f;

    [Header("战斗属性")]
    [SerializeField] private float _courage = 10f; // 勇气值（攻击力）
    [SerializeField] private float _willpower = 5f; // 意志力（护甲）
    [SerializeField] private float _moveSpeed = 100f; // 移动速度
    [SerializeField] private float _attackSpeed = 1.2f; // 攻击速度

    [Header("希望值")]
    [SerializeField] private float _hope = 10f;
    [SerializeField] private float _maxHope = 100f;
    [SerializeField] private float _minHope = 0f;

    [Header("状态判定")]
    [SerializeField] private float _statusCheckCooldown = 5f; // 状态检查冷却时间
    private float _lastStatusCheckTime;
    #endregion

    #region 属性访问器
    public float AwakeValue
    {
        get => _awakeValue;
        set
        {
            float oldValue = _awakeValue;
            _awakeValue = Mathf.Clamp(value, _minAwakeValue, _maxAwakeValue);
            if (oldValue != _awakeValue)
            {
                OnAwakeValueChanged?.Invoke(_awakeValue);
                CheckAwakeStatus();
            }
        }
    }

    public float MentalThreshold
    {
        get => _mentalThreshold;
        set
        {
            float oldValue = _mentalThreshold;
            _mentalThreshold = Mathf.Clamp(value, _minMentalThreshold, _maxMentalThreshold);
            if (oldValue != _mentalThreshold)
            {
                OnMentalThresholdChanged?.Invoke(_mentalThreshold);
                CheckMentalStatus();
            }
        }
    }

    public int SoulLevel
    {
        get => _soulLevel;
        private set
        {
            if (_soulLevel != value)
            {
                _soulLevel = Mathf.Clamp(value, 1, _maxSoulLevel);
                OnSoulLevelChanged?.Invoke(_soulLevel);
                OnLevelUp();
            }
        }
    }

    public float Courage
    {
        get => _courage;
        set
        {
            if (_courage != value)
            {
                _courage = value;
                OnCourageChanged?.Invoke(_courage);
            }
        }
    }

    public float Willpower
    {
        get => _willpower;
        set
        {
            if (_willpower != value)
            {
                _willpower = value;
                OnWillpowerChanged?.Invoke(_willpower);
            }
        }
    }

    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (_moveSpeed != value)
            {
                _moveSpeed = value;
                OnMoveSpeedChanged?.Invoke(_moveSpeed);
            }
        }
    }

    public float AttackSpeed
    {
        get => _attackSpeed;
        set
        {
            if (_attackSpeed != value)
            {
                _attackSpeed = value;
                OnAttackSpeedChanged?.Invoke(_attackSpeed);
            }
        }
    }

    public float Hope
    {
        get => _hope;
        set
        {
            float oldValue = _hope;
            _hope = Mathf.Clamp(value, _minHope, _maxHope);
            if (oldValue != _hope)
            {
                OnHopeChanged?.Invoke(_hope);
            }
        }
    }
    #endregion

    #region 初始化
    private void OnEnable()
    {
        InitializeDefaultValues();
    }

    /// <summary>
    /// 初始化默认值
    /// </summary>
    public void InitializeDefaultValues()
    {
        _awakeValue = 0f;
        _mentalThreshold = 30f;
        _soulLevel = 1;
        _courage = 10f;
        _willpower = 5f;
        _moveSpeed = 100f;
        _attackSpeed = 1.2f;
        _hope = 10f;
        _currentExp = 0f;
        _lastStatusCheckTime = 0f;
    }
    #endregion

    #region 游戏机制方法
    /// <summary>
    /// 计算实际伤害
    /// </summary>
    public float CalculateDamage(float baseSkillDamage = 0f, float weaponBonus = 0f)
    {
        float baseDamage = (_courage + baseSkillDamage + weaponBonus);
        float randomFactor = UnityEngine.Random.Range(0.9f, 1.1f);
        return baseDamage * randomFactor;
    }

    /// <summary>
    /// 计算伤害减免
    /// </summary>
    public float CalculateDamageReduction(float incomingDamage)
    {
        const float C = 50f; // 伤害减免系数
        float reductionPercentage = _willpower / (_willpower + C);
        return incomingDamage * (1 - reductionPercentage);
    }

    /// <summary>
    /// 增加经验值
    /// </summary>
    public void AddExperience(float expAmount)
    {
        _currentExp += expAmount;
        while (_currentExp >= _expToNextLevel && SoulLevel < _maxSoulLevel)
        {
            _currentExp -= _expToNextLevel;
            SoulLevel++;
        }
    }

    /// <summary>
    /// 升级时的属性提升
    /// </summary>
    private void OnLevelUp()
    {
        Courage += 2f;
        Willpower += 1f;
        MentalThreshold += 2f;
        // 这里可以添加更多升级奖励
    }

    /// <summary>
    /// 检查惊觉状态
    /// </summary>
    private void CheckAwakeStatus()
    {
        if (_awakeValue >= _maxAwakeValue)
        {
            OnFullyAwake();
        }
    }

    /// <summary>
    /// 完全惊醒时的处理
    /// </summary>
    private void OnFullyAwake()
    {
        // 降低心理阈值
        MentalThreshold -= 10f;
        // 根据希望值判定是否触发负面状态
        CheckForNegativeStatus();
    }

    /// <summary>
    /// 检查心理状态
    /// </summary>
    private void CheckMentalStatus()
    {
        if (Time.time - _lastStatusCheckTime < _statusCheckCooldown)
            return;

        _lastStatusCheckTime = Time.time;

        if (_mentalThreshold <= 0)
        {
            OnMentalBreakdown();
        }
        else if (_awakeValue < 40f) // 先决条件：惊觉值<40
        {
            // 每降低10点检查一次
            if (_mentalThreshold % 10f <= 0.01f)
            {
                CheckForStatusEffect();
            }
        }
    }

    /// <summary>
    /// 心理崩溃处理
    /// </summary>
    private void OnMentalBreakdown()
    {
        Debug.Log("触发Bad Ending - 心理崩溃");
        // 在这里添加游戏结束或特殊事件的处理
    }

    /// <summary>
    /// 检查状态效果
    /// </summary>
    private void CheckForStatusEffect()
    {
        float randomValue = UnityEngine.Random.value * 100f;
        if (randomValue <= _hope)
        {
            ApplyPositiveEffect();
        }
        else
        {
            ApplyNegativeEffect();
        }
    }

    /// <summary>
    /// 应用正面效果
    /// </summary>
    private void ApplyPositiveEffect()
    {
        // 这里可以实现具体的正面效果
        Debug.Log("触发正面效果");
    }

    /// <summary>
    /// 应用负面效果
    /// </summary>
    private void ApplyNegativeEffect()
    {
        // 这里可以实现具体的负面效果
        Debug.Log("触发负面效果");
    }

    /// <summary>
    /// 检查负面状态
    /// </summary>
    private void CheckForNegativeStatus()
    {
        float randomValue = UnityEngine.Random.value * 100f;
        if (randomValue > _hope)
        {
            ApplyNegativeEffect();
        }
    }
    #endregion
}
