using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : BaseSkill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked;

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot parryRestoreUnlockButton;
    [Range(0f,1f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool restoreUnlocked;

    [Header("Parry with mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
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
        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        parryRestoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    private void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
        {
            parryUnlocked = true;
        }
    }

    private void UnlockParryRestore()
    {
        if(parryRestoreUnlockButton.unlocked)
        {
            restoreUnlocked = true;
        }
    }

    private void UnlockParryWithMirage()
    {
        if(parryWithMirageUnlockButton.unlocked)
        {
            parryWithMirageUnlocked = true;
        }
    }

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if(parryWithMirageUnlocked)
        {
            SkillManager.instance.cloneSkill.CreateCloneWithDelay(_respawnTransform);
        }
    }
}