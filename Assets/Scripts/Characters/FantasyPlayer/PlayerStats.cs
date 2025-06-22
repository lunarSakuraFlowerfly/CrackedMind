
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    [SerializeField] private PlayerItemDrop playerItemDrop;
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }
    public override void Die()
    {
        base.Die();
        player.Die();
        playerItemDrop.GenerateDrops();
    }
    public override void OnEvasion()
    {
        base.OnEvasion();
        Debug.Log("OnEvasion");
    }
    public override void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();
        int _totalDamage = _fireDamage+_iceDamage+_lightningDamage + intelligence.GetValue();
        _totalDamage -= _targetStats.magicResistance.GetValue()+_targetStats.intelligence.GetValue()*3;
        _totalDamage = Mathf.Clamp(_totalDamage,0,int.MaxValue);
        _targetStats.TakeDamage(_totalDamage);

        if(Mathf.Max(_fireDamage,_iceDamage,_lightningDamage)<=0) return;

        if(player.skill.multipuleElementSkill.CanUseSkill())
        {
            AttemptToApplyAilments(_fireDamage,_iceDamage,_lightningDamage,_targetStats);
        }
    }
}

