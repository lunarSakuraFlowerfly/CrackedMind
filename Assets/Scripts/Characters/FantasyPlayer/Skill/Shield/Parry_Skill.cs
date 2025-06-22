using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : BaseSkill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked;

    [Header("Parry restore")]

    [Range(0f,1f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool restoreUnlocked;

    [Header("Parry with mirage")]
    public bool parryWithMirageUnlocked;

    public override void UseSkill()
    {
        base.UseSkill();
        if(restoreUnlocked)
        {
            int restoreAmount  = Mathf.RoundToInt(player.Stats.GetMaxHP()*restoreHealthPercentage);
            Debug.Log("Restore amount: " + restoreAmount);
            player.Stats.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Talent;
        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        parryUnlockButton.associatedSkill = this;
    }

    

    private void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
        {
            parryUnlocked = true;
        }
    }



    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if(parryWithMirageUnlocked)
        {
            SkillManager.instance.cloneSkill.CreateCloneWithDelay(_respawnTransform);
        }
    }
    protected override void CheckUnlock()
    {
        UnlockParry();
    }
}