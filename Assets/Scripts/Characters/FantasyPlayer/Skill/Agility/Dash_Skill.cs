using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : BaseSkill
{
    [Header("Dash")]
    public bool dashUnlocked;
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;

    [Header("Clone on dash")]
    public bool cloneOnDashUnlocked;
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;

    [Header("clone on arrival")]
    public bool cloneOnArrivalUnlocked;
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("在周围创造一个克隆体");
    }

    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Talent;
        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
        dashUnlockButton.associatedSkill = this;
        cloneOnDashUnlockButton.associatedSkill = this;
        cloneOnArrivalUnlockButton.associatedSkill = this;
    }
    private void UnlockDash()
    {
        Debug.Log("attempt to unlock dash");
        if(dashUnlockButton.unlocked)
        {
            Debug.Log("Dash unlocked");
            dashUnlocked = true;
        }
    }

    private void UnlockCloneOnDash()
    {
        Debug.Log("attempt to unlock clone on dash");
        if(cloneOnDashUnlockButton.unlocked)
        {
            Debug.Log("Clone on dash unlocked");
            cloneOnDashUnlocked = true;
        }
    }

    private void UnlockCloneOnArrival()
    {
        Debug.Log("attempt to unlock clone on arrival");
        if(cloneOnArrivalUnlockButton.unlocked)
        {
            Debug.Log("Clone on arrival unlocked");
            cloneOnArrivalUnlocked = true;
        }
    }

    public void CloneOnDash()
    {
        if(cloneOnDashUnlocked)
        {
            SkillManager.instance.cloneSkill.CreateCloneOnDashStart();
        }
    }
    public void CloneOnArrival()
    {
        if(cloneOnArrivalUnlocked)
        {
            SkillManager.instance.cloneSkill.CreateCloneOnDashEnd();
        }
    }
    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }
}