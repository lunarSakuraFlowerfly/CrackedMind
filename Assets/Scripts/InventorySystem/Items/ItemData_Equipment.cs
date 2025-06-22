using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


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

    [Header("Unique effect")]
    public float cooldown;
    public ItemEffect[] itemEffects;
    [TextArea]
    public string itemEffectDescription;
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
    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;
    private int descriptionLength;

    public void ExecuteItemEffect(Transform _enemyPosition)
    {
        foreach (ItemEffect effect in itemEffects)
        {
            effect.ExecuteEffect(_enemyPosition);
        }
    }
    private void OnValidate()
    {
        #if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);
        itemType = ItemType.Equipment;
        #endif
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

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;
        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");
        AddItemDescription(maxHP, "Health");
        AddItemDescription(armor, "Armor");
        AddItemDescription(magicResistance, "Magic Resistance");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Critical Chance");
        AddItemDescription(critPower, "Critical Power");
        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.Append("Unique: " + itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }
        if (descriptionLength < 5)
        {
            for (int i = descriptionLength; i < 5; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }
        if (itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(itemEffectDescription);
        }
        return sb.ToString();
    }

    private void AddItemDescription(int _value, string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);
            descriptionLength++;
        }
    }

    
}
