using UnityEngine;
using UnityEngine.UI;

public class MultipuleElementSkill : BaseSkill
{
    [Header("Skill Tree")]
    [SerializeField] private UI_SkillTreeSlot unlockSkillButton;
    [SerializeField] private bool canUseSkill;
    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Passive;
        unlockSkillButton.GetComponent<Button>().onClick.AddListener(UnlockSkill);
        unlockSkillButton.associatedSkill = this;
    }
    private void UnlockSkill()
    {
        if(unlockSkillButton.unlocked)
        {
            canUseSkill = true;
        }
    }
    public override bool CanUseSkill()
    {
        if(!canUseSkill) return false;
        return base.CanUseSkill();
    }
    protected override void CheckUnlock()
    {
        UnlockSkill();
    }
}