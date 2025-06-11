using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAnimatorEvents : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }
    private void AnimationTrigger()=>player.AnimationTrigger();

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                
                player.Stats?.DoDamage(enemy.Stats);
                Inventory.instance.GetEquipmentByType(EquipmentType.Weapon)?.ExecuteItemEffect(enemy.transform);

                ItemData_Equipment weaponData = Inventory.instance.GetEquipmentByType(EquipmentType.Weapon);
                if(weaponData != null)
                {
                    weaponData.ExecuteItemEffect(enemy.transform);
                }
            }
        }
    }


    private void ThrowSword()
    {
        player.skill.swordSkill.CreateSword();
    }
}
