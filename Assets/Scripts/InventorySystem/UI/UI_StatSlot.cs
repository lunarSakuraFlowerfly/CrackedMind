using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class UI_StatSlot : MonoBehaviour
{
    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;

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
    }
    public void UpdateStateValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if(playerStats!=null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();
        }
    }
}