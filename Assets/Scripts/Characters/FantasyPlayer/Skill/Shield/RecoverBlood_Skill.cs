using UnityEngine;
using UnityEngine.UI;
public class RecoverBlood_Skill : BaseSkill
{
    [SerializeField] private UI_SkillTreeSlot unlockSkillButton;
    [SerializeField] private bool canUseSkill;
    [SerializeField] private GameObject recoverBloodPrefab;
    [SerializeField] private float recoverBloodDuration;
    [SerializeField] private float recoverBloodInterval;
    [Range(0,100)]
    [SerializeField] private int recoverBloodMaxPercent;
    [Range(0,100)]
    [SerializeField] private int recoverBloodMinPercent;
    
    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Assignable;
        unlockSkillButton.GetComponent<Button>().onClick.AddListener(UnlockSkill);
        unlockSkillButton.associatedSkill = this;
    }
    public override void UseSkill()
    {
        //角色状态机转换
        player.stateMachine.ChangeState(player.healState);
        base.UseSkill();
        GameObject recoverBlood = Instantiate(recoverBloodPrefab,player.transform.position,Quaternion.identity);
        recoverBlood.transform.parent = player.transform;
        recoverBlood.GetComponent<RecoverBlood_Skill_Controller>().Setup(recoverBloodDuration,recoverBloodInterval,recoverBloodMaxPercent,recoverBloodMinPercent);
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