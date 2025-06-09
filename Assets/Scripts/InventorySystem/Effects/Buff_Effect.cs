using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item Effect/Buff")]
public class Buff_Effect : ItemEffect
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private int buffAmount;
    [SerializeField] private StatType statType;
    [SerializeField] private float duration;
    public override void ExecuteEffect(Transform _respawnPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        stats.IncreaseStatBy(buffAmount,duration,stats.GetStat(statType));
    }

  
}