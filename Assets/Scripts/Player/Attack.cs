using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("近战攻击")]
    public Vector2 attackSize = new Vector2(1f,1f); //攻击范围
    public float offsetX = 1f; //X轴偏移量
    public float offsetY = 1f; //Y轴偏移量
    private Vector2 AttackAreaPos; //攻击范围原点
    public float lightAttackMoveDis = 0.2f;
    public float heavyAttackMoveDis = 1f;

    public void LightAttackEvent(float isAttackAnimation)
    {
        switch (isAttackAnimation)
        {
            case 1:
                offsetX = -0.6f;
                offsetY = 0;
                attackSize = new Vector2(1, 1.3f);
                break;
            case 2:
                offsetX = -0.6f;
                offsetY = 0;
                attackSize = new Vector2(1, 1.3f);
                break;
            case 3:
                offsetX = -0.6f;
                offsetY = 0;
                attackSize = new Vector2(1, 1.3f);
                break;
            //case 4:
            //    offsetX = -0.6f;
            //    offsetY = 0;
            //    attackSize = new Vector2(1.2f, 1.5f);
            //    break;
        }
        PlayerController.Instance.LightAttackMove(lightAttackMoveDis);
        AttackAreaPos = transform.position;
        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(AttackAreaPos, attackSize,0);
        foreach(Collider2D hitCollider in hitColliders)
        {
            //TODO:获取敌人的受伤函数
            //hitCollider.GetComponent<Character>.TakeDamage();
        }
    }

    public void HeavyAttackEvent(float isAttackAnimation)
    {
        switch (isAttackAnimation)
        {
            case 4:
                offsetX = -0.6f;
                offsetY = 0;
                attackSize = new Vector2(1.2f, 1.5f);
                break;
        }
        AttackAreaPos = transform.position;
        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(AttackAreaPos, attackSize, 0);
        foreach(Collider2D hitCollider in hitColliders)
        {
            //TODO:获取敌人受伤函数
        }
    }

    public void HeavyAttackMove()
    {
        PlayerController.Instance.HeavyAttackMove(heavyAttackMoveDis);
    }
    public void HeavyAttackFalse()
    {
        PlayerController.Instance.HeavyAttackFalse();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AttackAreaPos,attackSize);
    }
}
