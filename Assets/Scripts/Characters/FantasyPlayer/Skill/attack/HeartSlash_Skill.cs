using UnityEngine;
using UnityEngine.UI;

public class HeartSlash_Skill : BaseSkill
{
    [SerializeField] private GameObject heartSlashPrefab;
    [SerializeField] private float effectDuration;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [Header("Skill Tree")]
    [SerializeField] private UI_SkillTreeSlot unlockSkillButton;
    [SerializeField] private bool canUseSkill;
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
        player.stateMachine.ChangeState(player.heartSlashState);
        base.UseSkill();
        int attackSpeedBuff = calculateAttackSpeedBuff();
        GameObject heartSlash = Instantiate(heartSlashPrefab, player.transform.position, Quaternion.identity);
        heartSlash.transform.SetParent(player.transform);
        heartSlash.transform.localPosition = Vector3.zero;
        heartSlash.GetComponent<HeartSlash_Skill_Controller>().SetHeartSlash(effectDuration,attackSpeedBuff,fadeInDuration,fadeOutDuration);
    }
    public override bool CanUseSkill()
    {
        if(!canUseSkill) return false;
        return base.CanUseSkill();
    }
    private void UnlockSkill()
    {
        if(unlockSkillButton.unlocked)
        {
            canUseSkill = true;
        }
    }
    private int calculateAttackSpeedBuff()
    {
        //根据目前的敏捷值、智力和体力计算攻击速度提升的百分比
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        int strengthSpeedBuff = playerStats.strength.GetValue();
        int agilitySpeedBuff = playerStats.agility.GetValue();
        int vitalitySpeedBuff = playerStats.vitality.GetValue();
        int totalSpeedBuff = strengthSpeedBuff + agilitySpeedBuff + vitalitySpeedBuff+30;
        return totalSpeedBuff;
    }
    protected override void CheckUnlock()
    {
        UnlockSkill();
    }
}