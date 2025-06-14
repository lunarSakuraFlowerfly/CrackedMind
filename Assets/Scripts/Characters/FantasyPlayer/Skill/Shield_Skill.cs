using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Shield_Skill : BaseSkill
{

    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float shieldDuration;

    private Shield_Skill_Controller shieldController;

    [SerializeField] private UI_SkillTreeSlot unlockShieldButton;
    public bool shieldUnlocked{get;private set;}

    protected override void Start()
    {
        base.Start();
        unlockShieldButton.GetComponent<Button>().onClick.AddListener(UnlockShield);
    }
    private void UnlockShield()
    {
        if(unlockShieldButton.unlocked)
        {
            shieldUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        int shieldValue = CalculateShieldValue();
        GameObject newShield = Instantiate(shieldPrefab,player.transform.position,Quaternion.identity);
        //将此护盾设置到玩家身上
        newShield.transform.SetParent(player.transform);
        newShield.transform.localPosition = Vector3.zero;
        shieldController = newShield.GetComponent<Shield_Skill_Controller>();
        shieldController.SetupShield(shieldValue,shieldDuration,growSpeed,shrinkSpeed);
    }

    private int CalculateShieldValue()
    {
        //根据玩家的护甲、魔法抗性、最大生命、体力来算护盾值
        CharacterStats stats = player.GetComponent<CharacterStats>();
        if(stats == null)
        {
            Debug.LogError("玩家没有CharacterStats组件");
            return 0;
        }
        //基础护盾值=护甲+魔抗
        float baseShield = stats.armor.GetValue() + stats.magicResistance.GetValue();
        //生命值加成=最大生命值的百分之20
        float healthBonus = stats.GetMaxHP()*0.2f;
        //体力加成
        float vitalityBonus = stats.vitality.GetValue()*2;
        //智力加成
        float intelligenceBonus = stats.intelligence.GetValue()*1.5f;
        //总护盾值
        float totalShield = baseShield + healthBonus + vitalityBonus + intelligenceBonus;

        return Mathf.RoundToInt(totalShield);
    }
}