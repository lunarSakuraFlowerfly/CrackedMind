
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum SkillType
{
    Assignable,
    Passive,
    Talent
}

public enum SkillSlot
{
    None = -1,
    Slot1 = 0,
    Slot2 = 1,
    Slot3 = 2,
    Slot4 = 3,
}
[System.Serializable]
public class SkillSlotUI
{
    public Image skillSlotImage;     // 背景图片，显示技能图标
    public Image skillSlotBackground; // 如果需要的话
    [HideInInspector] public BaseSkill assignedSkill; // 当前分配的技能
    [HideInInspector] public Sprite originalIcon;     // 原始技能图标
}

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [Header("技能槽位UI")]
    [SerializeField] private List<SkillSlotUI> skillSlots = new List<SkillSlotUI>(); // 4个技能槽位
    [SerializeField] private Sprite defaultSkillIcon; // 默认技能图标（空槽位时显示）
    private SkillManager skillManager;

    [Header("souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;


    private void Start()
    {
        if(playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthUI;
        }
        
        // ✅ 延迟获取 SkillManager 确保初始化完成
        Invoke(nameof(InitializeSkillSystem), 0.1f);
        
        UpdateHealthUI();
        UpdateCurrencyUI();
        
    }

    private void UpdateCurrencyUI()
    {
        currentSouls.text = PlayerManager.instance.GetCurrency().ToString();
    }

    /// <summary>
    /// ✅ 延迟初始化技能系统
    /// </summary>
    private void InitializeSkillSystem()
    {
        skillManager = SkillManager.instance;
        
        if(skillManager != null)
        {
            skillManager.OnSkillAssigned += OnSkillAssigned;
            skillManager.OnSkillUnassigned += OnSkillUnassigned;
            Debug.Log("UI_InGame: 已注册技能分配事件监听");
            
            // 初始化技能槽位显示
            InitializeSkillSlots();
        }
        else
        {
            Debug.LogError("SkillManager.instance 为空！");
        }
    }

    private void OnDestroy()
    {
        if(skillManager != null)
        {
            skillManager.OnSkillAssigned -= OnSkillAssigned;
            skillManager.OnSkillUnassigned -= OnSkillUnassigned;
        }
    }


    void Update()
    {
        UpdateSoulsUI();
       
            // 检测技能按键输入
        for(int i = 0; i < 4; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if(Input.GetKeyDown(key))
            {
                SkillSlot slot = (SkillSlot)i;
                BaseSkill skill = skillManager?.GetSkillInSlot(slot);
                if(skill != null && skill.CanUseSkill())
                {
                    // 播放技能使用音效
                    if (AudioManager.instance != null)
                        AudioManager.instance.PlaySFX(4, null);
                        
                    StartCooldownDisplay(i);
                }
            }
        }
        UpdateAllCooldowns();
    }

    private void UpdateSoulsUI()
    {
        if(soulsAmount<PlayerManager.instance.GetCurrency())
        {
            soulsAmount += increaseRate * Time.deltaTime;
        }
        else
        {
            soulsAmount = PlayerManager.instance.GetCurrency();
        }
        currentSouls.text = ((int)soulsAmount).ToString();
    }


    /// <summary>
    /// 初始化技能槽位显示
    /// </summary>
    private void InitializeSkillSlots()
    {
        Debug.Log($"初始化技能槽位，共有 {skillSlots.Count} 个槽位");
        
        for(int i = 0; i < skillSlots.Count; i++)
        {
            SkillSlot slot = (SkillSlot)i;
            BaseSkill assignedSkill = skillManager?.GetSkillInSlot(slot);
            UpdateSkillSlotDisplay(i, assignedSkill);
        }
    }

    /// <summary>
    /// 技能分配事件处理
    /// </summary>
    private void OnSkillAssigned(SkillSlot slot, BaseSkill skill)
    {
        int slotIndex = (int)slot;
        
        if(slotIndex >= 0 && slotIndex < skillSlots.Count)
        {
            UpdateSkillSlotDisplay(slotIndex, skill);
        }
    }

    /// <summary>
    /// 技能取消分配事件处理
    /// </summary>
    private void OnSkillUnassigned(SkillSlot slot)
    {
        int slotIndex = (int)slot;
        if(slotIndex >= 0 && slotIndex < skillSlots.Count)
        {
            UpdateSkillSlotDisplay(slotIndex, null);
            Debug.Log($"UI更新：槽位 {slot} 技能已取消分配");
        }
    }

    /// <summary>
    /// ✅ 更新技能槽位显示
    /// </summary>
    private void UpdateSkillSlotDisplay(int slotIndex, BaseSkill skill)
    {
        if(slotIndex < 0 || slotIndex >= skillSlots.Count) 
        {
            Debug.LogError($"UpdateSkillSlotDisplay: 无效的槽位索引 {slotIndex}");
            return;
        }

        SkillSlotUI slotUI = skillSlots[slotIndex];
        if(slotUI == null)
        {
            return;
        }

        if(slotUI.skillSlotImage == null)
        {
            return;
        }

        slotUI.assignedSkill = skill;

        if(skill != null)
        {
            // 获取并设置技能图标
            Sprite skillIcon = GetSkillIconFromSkillTree(skill);
            if(skillIcon != null)
            {
                slotUI.skillSlotImage.sprite = skillIcon;
                slotUI.skillSlotBackground.sprite = skillIcon;
            }
            else
            {
                slotUI.originalIcon = defaultSkillIcon;
                slotUI.skillSlotImage.sprite = defaultSkillIcon;
            }
        }
        else
        {
            slotUI.originalIcon = defaultSkillIcon;
            slotUI.skillSlotImage.sprite = defaultSkillIcon;
            slotUI.skillSlotBackground.sprite = defaultSkillIcon;
        }

        // 重置冷却显示
        slotUI.skillSlotImage.fillAmount = 0f;
        
        // ✅ 强制刷新UI
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// 从技能树UI获取技能图标
    /// </summary>
    private Sprite GetSkillIconFromSkillTree(BaseSkill skill)
    {
        UI_SkillTreeSlot[] skillTreeSlots = FindObjectsOfType<UI_SkillTreeSlot>(true);
        Debug.Log($"找到 {skillTreeSlots.Length} 个技能树槽位");
        
        foreach(var treeSlot in skillTreeSlots)
        {
            if(treeSlot.associatedSkill == skill)
            {
                Image treeSlotImage = treeSlot.GetComponent<Image>();
                if(treeSlotImage != null && treeSlotImage.sprite != null)
                {
                    Debug.Log($"找到技能 {skill.name} 的图标: {treeSlotImage.sprite.name}");
                    return treeSlotImage.sprite;
                }
            }
        }
        
        Debug.LogWarning($"未找到技能 {skill.name} 对应的技能树槽位");
        return null;
    }

    private void StartCooldownDisplay(int slotIndex)
    {
        if(slotIndex < 0 || slotIndex >= skillSlots.Count) return;
        
        SkillSlotUI slotUI = skillSlots[slotIndex];
        if(slotUI.skillSlotImage != null)
        {
            slotUI.skillSlotImage.fillAmount = 1f;
        }
    }

    private void UpdateAllCooldowns()
    {
        for(int i = 0; i < skillSlots.Count; i++)
        {
            SkillSlotUI slotUI = skillSlots[i];
            if(slotUI.assignedSkill != null && slotUI.skillSlotBackground != null)
            {
                if(slotUI.assignedSkill.IsOnCooldown)
                {
                    float progress = slotUI.assignedSkill.CooldownTimer / slotUI.assignedSkill.cooldown;
                    Debug.Log("progress: " + progress);
                    slotUI.skillSlotBackground.fillAmount = progress;
                }
                else
                {
                    slotUI.skillSlotBackground.fillAmount = 0f;
                }
            }
        }
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHP();
        slider.value = playerStats.GetCurrentHP();
    }
    

    
}