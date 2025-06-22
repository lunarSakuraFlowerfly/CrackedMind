using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour,ISaveManager
{
    public static SkillManager instance;

    [Header("技能槽分配")]
    [SerializeField] private BaseSkill[] assignedSkills = new BaseSkill[4];

    public Dash_Skill dashSkill{get;private set;}
    public Clone_Skill cloneSkill{get;private set;}
    public Sword_Skill swordSkill{get;private set;}
    public Blackhole_Skill blackholeSkill{get;private set;}
    public Crystal_Skill crystalSkill{get;private set;}
    public Parry_Skill parrySkill{get;private set;}
    public Shield_Skill shieldSkill{get;private set;}
    public HeartSlash_Skill heartSlashSkill{get;private set;}
    public RagingSurge_Skill ragingSurgeSkill{get;private set;}
    public Dawnbreaker_Skill dawnbreakerSkill{get;private set;}
    public MultipuleElementSkill multipuleElementSkill{get;private set;}
    public RecoverBlood_Skill recoverBloodSkill{get;private set;}
    public PhantomAttack_Skill phantomAttackSkill{get;private set;}

    //技能分类列表
    private List<BaseSkill> allSkills = new List<BaseSkill>();
    private List<BaseSkill> assignableSkills = new List<BaseSkill>();
    private List<BaseSkill> passiveSkills = new List<BaseSkill>();
    private List<BaseSkill> talentSkills = new List<BaseSkill>();
    //事件
    public event Action<SkillSlot,BaseSkill> OnSkillAssigned;
    public event Action<SkillSlot> OnSkillUnassigned;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    /*
    private void Start()
    {
        InitializeSkills();
        CategorizeSkills();
    }
    */
    private void InitializeSkills()
    {
        dashSkill = GetComponent<Dash_Skill>();
        cloneSkill = GetComponent<Clone_Skill>();
        swordSkill = GetComponent<Sword_Skill>();
        blackholeSkill = GetComponent<Blackhole_Skill>();
        crystalSkill = GetComponent<Crystal_Skill>();
        parrySkill = GetComponent<Parry_Skill>();
        shieldSkill = GetComponent<Shield_Skill>();
        heartSlashSkill = GetComponent<HeartSlash_Skill>();
        ragingSurgeSkill = GetComponent<RagingSurge_Skill>();
        dawnbreakerSkill = GetComponent<Dawnbreaker_Skill>();
        multipuleElementSkill = GetComponent<MultipuleElementSkill>();
        recoverBloodSkill = GetComponent<RecoverBlood_Skill>();
        phantomAttackSkill = GetComponent<PhantomAttack_Skill>();

        //添加一些技能到总列表
        allSkills.AddRange(new BaseSkill[]{
            dashSkill,
            swordSkill,
            parrySkill,
            shieldSkill,
            heartSlashSkill,
            ragingSurgeSkill,
            dawnbreakerSkill,
            multipuleElementSkill,
            recoverBloodSkill,
            phantomAttackSkill,
        });
    }
    private void CategorizeSkills()
    {
        assignableSkills.Clear();
        passiveSkills.Clear();
        talentSkills.Clear();

        foreach(var skill in allSkills)
        {
            if(skill==null) return;
            switch(skill.Skill_Type)
            {
                case SkillType.Assignable:
                    assignableSkills.Add(skill);
                    break;
                case SkillType.Passive:
                    passiveSkills.Add(skill);
                    break;
                case SkillType.Talent:
                    talentSkills.Add(skill);
                    break;
                default:
                    break;
            }
        }
    }


public bool AssignSkillToSlot(BaseSkill skill, SkillSlot slot)
{
    
    if(skill == null || slot == SkillSlot.None)
    {
        return false;
    }

    if(skill.Skill_Type != SkillType.Assignable)
    {
        return false;
    }
    
    int slotIndex = (int)slot;
    
    // 如果槽位已有技能，先取消分配
    if(assignedSkills[slotIndex] != null)
    {
        UnassignSkillFromSlot(slot);
    }
    
    assignedSkills[slotIndex] = skill;
    
    // 检查事件是否有监听者
    if(OnSkillAssigned != null)
    {
    
        OnSkillAssigned.Invoke(slot, skill);
    }
    return true;
}
    public void UnassignSkillFromSlot(SkillSlot slot)
    {
        if(slot == SkillSlot.None) return;
        int slotIndex = (int)slot;
        BaseSkill previousSkill = assignedSkills[slotIndex];
        if(previousSkill != null)
        {
            assignedSkills[slotIndex] = null;
            OnSkillUnassigned?.Invoke(slot);
            Debug.Log($"技能槽{slot}已取消分配");
        }
    }

    public BaseSkill GetSkillInSlot(SkillSlot slot)
    {
        if(slot == SkillSlot.None) return null;
        int slotIndex = (int)slot;
        return assignedSkills[slotIndex];
    }

    public BaseSkill GetSkillByKeyboard(KeyCode keyCode)
    {
        return keyCode switch
        {
            KeyCode.Alpha1 => GetSkillInSlot(SkillSlot.Slot1),
            KeyCode.Alpha2 => GetSkillInSlot(SkillSlot.Slot2),
            KeyCode.Alpha3 => GetSkillInSlot(SkillSlot.Slot3),
            KeyCode.Alpha4 => GetSkillInSlot(SkillSlot.Slot4),
            _ => null,
        };
    }

    public void UseSkillInSlot(SkillSlot slot)
    {
        BaseSkill skill = GetSkillInSlot(slot);
        if(skill != null&&skill.CanUseSkill())
        {
            skill.UseSkill();
        } 
    }

    public List<BaseSkill> GetAssignableSkills()
    {
        return new List<BaseSkill>(assignableSkills);
    }

    public (List<BaseSkill> assignable,List<BaseSkill> passive,List<BaseSkill> talent) GetAllSkills()
    {
        return (assignableSkills,passiveSkills,talentSkills);
    }

    public void SaveData(ref GameData _data)
    {
        _data.assignedSkills.Clear();
        for(int i =0;i<4;++i)
        {
            if(assignedSkills[i] != null)
            {
                _data.assignedSkills.Add(assignedSkills[i].GetType().Name);
            }
            else
            {
                _data.assignedSkills.Add("");
            }
        }
    }

    private IEnumerator DelayedSkillAssignEvent(SkillSlot slot,BaseSkill skill)
    {
        yield return new WaitForSeconds(0.2f);
        OnSkillAssigned?.Invoke(slot,skill);
    }

    public void LoadData(GameData _data)
    {
        InitializeSkills();
        CategorizeSkills();
        if(_data.assignedSkills==null) return;
        for(int i = 0;i<assignedSkills.Length;++i)
        {
            assignedSkills[i] = null;
        }
        for(int i = 0;i<_data.assignedSkills.Count&&i<4;++i)
        {
            string skillName = _data.assignedSkills[i];
            if(!string.IsNullOrEmpty(skillName))
            {
                BaseSkill skill = FindSkillByName(skillName);
                if(skill != null)
                {
                    assignedSkills[i] = skill;
                    StartCoroutine(DelayedSkillAssignEvent((SkillSlot)i,skill));
                }
            }
        }
    }
    // 根据名称查找技能
    private BaseSkill FindSkillByName(string skillName)
    {
        switch(skillName)
        {
            case "Dash_Skill": return dashSkill;
            case "Sword_Skill": return swordSkill;
            case "Parry_Skill": return parrySkill;
            case "Shield_Skill": return shieldSkill;
            case "HeartSlash_Skill": return heartSlashSkill;
            case "RagingSurge_Skill": return ragingSurgeSkill;
            case "Dawnbreaker_Skill": return dawnbreakerSkill;
            case "MultipuleElementSkill": return multipuleElementSkill;
            case "RecoverBlood_Skill": return recoverBloodSkill;
            case "PhantomAttack_Skill": return phantomAttackSkill;
            default: return null;
        }
    }

}

