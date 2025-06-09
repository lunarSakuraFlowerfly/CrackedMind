using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    [Header("level detial")]
    [SerializeField] private int level;

    [SerializeField] private float percentageModifier = .4f;
    [SerializeField] private ItemDrop itemDrop;
    protected override void Start()
    {
        ApplyModifier();
        base.Start();
        enemy = GetComponent<Enemy>();
        itemDrop = GetComponent<ItemDrop>();
    }

    private void Modify(Stat _stat)
    {
        for(int i =1;i<level;i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    private void ApplyModifier()
    {
        Modify(damage);
        Modify(maxHP);
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }
    public override void Die()
    {
        base.Die();
        enemy.Die();
        itemDrop.GenerateDrops();
    }
 
}

