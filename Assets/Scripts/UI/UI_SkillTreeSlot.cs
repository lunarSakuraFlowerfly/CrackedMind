using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,ISaveManager
{
    private UI ui;
    private Image skillImage;
    
    [Header("技能设置")]
    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;
    
    [Header("关联的技能脚本")]
    [SerializeField] public BaseSkill associatedSkill; // 关联的技能脚本

    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();
        skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }

    public void UnlockSkillSlot()
    {
        // 播放点击音效
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(4, null);
            
        for(int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if(shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("cannot unlock skill");
                return;
            }
        }

        for(int i = 0; i < shouldBeLocked.Length; i++)
        {
            if(shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("cannot unlock skill");
                return;
            }
        }

        if(!PlayerManager.instance.HaveEnoughCurrency(skillPrice))
        {
            Debug.Log("Not enough currency");
            return;
        }

        unlocked = true;
        skillImage.color = Color.white;
    }

    /// <summary>
    /// 处理鼠标点击事件（左键解锁，右键分配）
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && unlocked)
        {
            // 播放右键点击音效
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(4, null);
                
            // 右键点击已解锁技能，显示分配界面
            ShowSkillAssignSlot();
        }
        // 左键点击在 Button.onClick 中处理（解锁技能）
    }

    /// <summary>
    /// 显示技能分配界面
    /// </summary>
    private void ShowSkillAssignSlot()
    {
        if(associatedSkill == null)
        {
            Debug.LogError($"技能槽 {skillName} 没有关联的技能脚本");
            return;
        }

        // 检查技能是否可分配
        if(associatedSkill.Skill_Type != SkillType.Assignable)
        {
            Debug.Log($"技能 {skillName} 不可分配到技能槽");
            return;
        }

        // 计算显示位置
        Vector2 mousePosition = Input.mousePosition;
        Vector2 displayPosition = CalculateAssignSlotPosition(mousePosition);

        // 通过UI管理器显示技能分配界面
        if(ui != null)
        {
            ui.ShowAssignSkillSlot(associatedSkill, displayPosition);
        }
        else
        {
            Debug.LogError("找不到 UI 组件");
        }
    }

    /// <summary>
    /// 计算技能分配界面的显示位置
    /// </summary>
    private Vector2 CalculateAssignSlotPosition(Vector2 mousePosition)
    {
        float xOffset = 0;
        float yOffset = 0;
        
        // 根据鼠标位置调整显示位置，避免超出屏幕
        if(mousePosition.x > Screen.width * 0.6f) // 鼠标在屏幕右侧
            xOffset = -40; // 向左偏移
        else
            xOffset = 40;  // 向右偏移

        if(mousePosition.y > Screen.height * 0.6f) // 鼠标在屏幕上方
            yOffset = -60; // 向下偏移
        else
            yOffset = 60;  // 向上偏移

        return new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
    }

    /// <summary>
    /// 获取关联的技能（供其他脚本调用）
    /// </summary>
    public BaseSkill GetAssociatedSkill()
    {
        return associatedSkill;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string tooltipText = skillDescription;
        
        // 如果技能已解锁且可分配，添加右键提示
        if(unlocked && associatedSkill != null && associatedSkill.Skill_Type == SkillType.Assignable)
        {
            tooltipText += "\n\n右键点击分配到技能槽";
        }
        
        ui.skillToolTip.ShowToolTip(tooltipText, skillName, skillPrice);


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            unlocked = value;
        }
    }

}