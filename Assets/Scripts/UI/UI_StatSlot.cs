using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class UI_StatSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private UI ui;
    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        statNameText.text ="Stat - " + statName;

        if(statNameText!=null)
        {
            statNameText.text = statName;
        }
        
    }
    private void Start()
    {
        UpdateStateValueUI();
        ui = GetComponentInParent<UI>();
    }
    public void UpdateStateValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if(playerStats!=null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();

            switch(statType)
            {
                case StatType.Health:
                    statValueText.text = playerStats.GetMaxHP().ToString();
                    break;
                case StatType.Damage:
                    statValueText.text = (playerStats.damage.GetValue()+playerStats.strength.GetValue()).ToString();
                    break;
                case StatType.CritPower:
                    statValueText.text = (playerStats.critPower.GetValue()+playerStats.strength.GetValue()).ToString();
                    break;
                case StatType.CritChance:
                    statValueText.text = (playerStats.critChance.GetValue()+playerStats.agility.GetValue()).ToString();
                    break;
                case StatType.FireDamage:
                    statValueText.text = (playerStats.fireDamage.GetValue()+playerStats.intelligence.GetValue()).ToString();
                    break;
                case StatType.IceDamage:
                    statValueText.text = (playerStats.iceDamage.GetValue()+playerStats.intelligence.GetValue()).ToString();
                    break;
                case StatType.LightningDamage:
                    statValueText.text = (playerStats.lightningDamage.GetValue()+playerStats.intelligence.GetValue()).ToString();
                    break;
                case StatType.Armor:
                    statValueText.text = playerStats.armor.GetValue().ToString();
                    break;
                case StatType.MagicResistance:
                    statValueText.text = (playerStats.magicResistance.GetValue()+playerStats.intelligence.GetValue()*3).ToString();
                    break;
                case StatType.Evasion:
                    statValueText.text = (playerStats.evasion.GetValue()+playerStats.agility.GetValue()).ToString();
                    break;
                case StatType.Agility:
                    statValueText.text = playerStats.agility.GetValue().ToString();
                    break;
                case StatType.Intelligence:
                    statValueText.text = playerStats.intelligence.GetValue().ToString();
                    break;
                case StatType.Vitality:
                    statValueText.text = playerStats.vitality.GetValue().ToString();
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);
    }
    public void OnPointerExit(PointerEventData eventData)   
    {
        ui.statToolTip.HideToolTip();
    }
}