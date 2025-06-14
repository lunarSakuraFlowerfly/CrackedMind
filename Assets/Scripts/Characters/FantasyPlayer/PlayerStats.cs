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
}

