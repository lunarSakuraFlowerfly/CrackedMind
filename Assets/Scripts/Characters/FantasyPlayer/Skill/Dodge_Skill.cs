using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : BaseSkill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    public bool dodgeUnlocked;
    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodgeButton;
    public bool mirageDodgeUnlocked;

    protected override void Start()
    {
        base.Start();
        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    private void UnlockDodge()
    {
        if(unlockDodgeButton.unlocked)
        {
            player.Stats.evasion.AddModifier(10);
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodgeButton.unlocked)
        {
            mirageDodgeUnlocked = true;
        }
    }
    
    public void CreateMirageDodge()
    {
        if(mirageDodgeUnlocked)
        {
            SkillManager.instance.cloneSkill.CreateClone(player.transform,new Vector3(0,0));
        }
    }
}