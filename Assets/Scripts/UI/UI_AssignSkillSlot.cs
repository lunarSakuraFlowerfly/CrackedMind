using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_AssignSkillSlot : MonoBehaviour
{
    [SerializeField] private GameObject skillSlotParent;
    private List<Button> skillSlotButtons = new List<Button>(); // 分配选择按钮
    private BaseSkill currentSkillToAssign; // 当前要分配的技能
    private SkillManager skillManager;
    private RectTransform rectTransform;

    private void Start()
    {
        skillManager = SkillManager.instance;
        rectTransform = GetComponent<RectTransform>();
        FindAllSkillSlots();
    }

    private void Update()
    {
        // 检测鼠标左键点击空白区域隐藏
        if(Input.GetMouseButtonDown(0))
        {
            CheckClickOutside();
        }
    }

    /// <summary>
    /// 查找所有分配选择按钮
    /// </summary>
    private void FindAllSkillSlots()
    {
        skillSlotButtons.Clear();
        
        for(int i = 0; i < skillSlotParent.transform.childCount; i++)
        {
            GameObject child = skillSlotParent.transform.GetChild(i).gameObject;
            Button button = child.GetComponent<Button>();
            
            if(button != null)
            {
                skillSlotButtons.Add(button);
                
                // 为每个分配按钮添加点击事件
                int slotIndex = i; // 避免闭包问题
                button.onClick.AddListener(() => {
                    // 播放点击音效
                    if (AudioManager.instance != null)
                        AudioManager.instance.PlaySFX(4, null);
                    AssignSkillToSlot(slotIndex);
                });
            }
        }
        
        Debug.Log($"找到 {skillSlotButtons.Count} 个技能分配选择按钮");
    }

    /// <summary>
    /// 检查是否点击在UI外部
    /// </summary>
    private void CheckClickOutside()
    {
        Vector2 localMousePosition;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            null,
            out localMousePosition))
        {
            if(!rectTransform.rect.Contains(localMousePosition))
            {
                HideSlotSelection();
            }
        }
    }

    /// <summary>
    /// 显示技能分配选择界面
    /// </summary>
    /// <param name="skill">要分配的技能</param>
    /// <param name="position">显示位置</param>
    public void ShowSlotSelection(BaseSkill skill, Vector2 position)
    {
        if(skill == null)
        {
            Debug.LogError("技能为空,无法显示槽位选择");
            return;
        }
        
        if(skill.Skill_Type != SkillType.Assignable)
        {
            Debug.LogError($"技能 {skill.name} 不是可分配类型");
            return;
        }
        
        currentSkillToAssign = skill;
        
        // 设置界面位置
        transform.position = position;
        
        // 确保界面在屏幕范围内
        ClampToScreen();
        
        gameObject.SetActive(true);
        
        // 更新按钮状态显示
        UpdateSlotStates();
        
        Debug.Log($"显示技能分配选择：{skill.name} 位置：{position}");
    }

    /// <summary>
    /// 显示技能分配选择界面（重载方法，保持兼容性）
    /// </summary>
    /// <param name="skill">要分配的技能</param>
    public void ShowSlotSelection(BaseSkill skill)
    {
        Vector2 centerPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        ShowSlotSelection(skill, centerPosition);
    }

    /// <summary>
    /// 确保界面不超出屏幕边界
    /// </summary>
    private void ClampToScreen()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 worldPosition = transform.position;
        
        // 获取界面的尺寸
        Vector2 sizeDelta = rect.sizeDelta;
        float halfWidth = sizeDelta.x * 0.5f;
        float halfHeight = sizeDelta.y * 0.5f;
        
        // 限制在屏幕范围内
        worldPosition.x = Mathf.Clamp(worldPosition.x, halfWidth, Screen.width - halfWidth);
        worldPosition.y = Mathf.Clamp(worldPosition.y, halfHeight, Screen.height - halfHeight);
        
        transform.position = worldPosition;
    }

    /// <summary>
    /// 隐藏技能分配选择界面
    /// </summary>
    public void HideSlotSelection()
    {
        gameObject.SetActive(false);
        currentSkillToAssign = null;
    }

    /// <summary>
    /// 分配技能到指定槽位
    /// </summary>
    private void AssignSkillToSlot(int slotIndex)
    {
        if(currentSkillToAssign == null)
        {
            Debug.LogError("没有要分配的技能");
            return;
        }

        if(slotIndex < 0 || slotIndex >= 4)
        {
            Debug.LogError($"无效的槽位索引：{slotIndex}");
            return;
        }

        SkillSlot targetSlot = (SkillSlot)slotIndex;
        bool success = skillManager.AssignSkillToSlot(currentSkillToAssign, targetSlot);
        
        if(success)
        {
            Debug.Log($"技能 {currentSkillToAssign.name} 成功分配到槽位 {targetSlot}");
            HideSlotSelection();
        }
        else
        {
            Debug.LogError($"技能分配失败");
        }
    }

    /// <summary>
    /// 更新分配选择按钮的状态显示
    /// </summary>
    private void UpdateSlotStates()
    {
        for(int i = 0; i < skillSlotButtons.Count && i < 4; i++)
        {
            SkillSlot slot = (SkillSlot)i;
            BaseSkill assignedSkill = skillManager.GetSkillInSlot(slot);
            Button button = skillSlotButtons[i];
            
        }
    }

    /// <summary>
    /// 获取按键文本
    /// </summary>
    private string GetKeyText(int slotIndex)
    {
        return slotIndex switch
        {
            0 => "1键",
            1 => "2键", 
            2 => "3键",
            3 => "4键",
            _ => "未知"
        };
    }
}