using UnityEngine;

public enum EquipmentType
{
    Weapon=0,
    Armor=1,
    Amulet=2,
    Flask=3
}

[CreateAssetMenu(fileName = "New Equipment Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemDataSO
{
    public EquipmentType equipmentType;

    public float cooldown;

    public ItemEffect[] itemEffects;
    [Header("Major stats")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;
    [Header("Defensive stats")]
    public int maxHP;
    public int armor;
    public int magicResistance;
    public int evasion;
    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;
    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    private void OnValidate()
    {
        itemType = ItemType.Equipment;
    }
    public void ExecuteItemEffect(Transform _enemyPosition)
    {
        foreach(ItemEffect effect in itemEffects)
        {
            effect.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.maxHP.AddModifier(maxHP);
        playerStats.armor.AddModifier(armor);
        playerStats.magicResistance.AddModifier(magicResistance);
        playerStats.evasion.AddModifier(evasion);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }
    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.maxHP.RemoveModifier(maxHP);
        playerStats.armor.RemoveModifier(armor);
        playerStats.magicResistance.RemoveModifier(magicResistance);
        playerStats.evasion.RemoveModifier(evasion);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);

    }
}
