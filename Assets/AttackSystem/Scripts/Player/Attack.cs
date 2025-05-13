using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("???????")]
    public Vector2 attackSize = new Vector2(1f,1f); //??????¦¶
    public float offsetX = 1f; //X???????
    public float offsetY = 1f; //Y???????
    private Vector2 AttackAreaPos; //??????¦¶???
    public float lightAttackMoveDis = 0.2f;
    public float heavyAttackMoveDis = 1f;
    

    public void LightAttackEvent(float isAttackAnimation)
    {
        switch (isAttackAnimation)
        {
            case 1:
                offsetX = 0.6f*PlayerController.Instance.lookAt.x;
                offsetY = 0.6f * PlayerController.Instance.lookAt.y;
                attackSize = new Vector2(1, 1.3f);
                break;
            case 2:
                offsetX = PlayerController.Instance.lookAt.x*0.6f;
                offsetY = 0.6f * PlayerController.Instance.lookAt.y;
                attackSize = new Vector2(1, 1.3f);
                break;
            case 3:
                offsetX = PlayerController.Instance.lookAt.x * 0.6f;
                offsetY = 0.6f * PlayerController.Instance.lookAt.y;
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
            //TODO:???????????????
            //hitCollider.GetComponent<Character>.TakeDamage();
        }
    }

    public void HeavyAttackEvent(float isAttackAnimation)
    {
        switch (isAttackAnimation)
        {
            case 4:
                offsetX = PlayerController.Instance.lookAt.x * 0.6f;
                offsetY = 0.6f * PlayerController.Instance.lookAt.y;
                attackSize = new Vector2(1.2f, 1.5f);
                break;
        }
        AttackAreaPos = transform.position;
        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(AttackAreaPos, attackSize, 0);
        foreach(Collider2D hitCollider in hitColliders)
        {
            //TODO:??????????????
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

    //???????
    
    #region HeartSlash

    public void HeartSlashEvent()
    {
        Debug.Log("????HeartSlashEvent????§¹??");
        offsetX = PlayerController.Instance.lookAt.x * 0.6f;
        offsetY = 0.6f * PlayerController.Instance.lookAt.y;
        attackSize = new Vector2(1, 1.3f);
        AttackAreaPos = transform.position;
        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(AttackAreaPos,attackSize,0);

        float attackValue = 0;
        switch (PlayerController.Instance.playerSkillSO.HeartSlash.skillLevel)
        {
            case 1:
                attackValue = PlayerController.Instance.playerSO.attackValue.value * 1.2f;
                break;
            case 2:
                attackValue = PlayerController.Instance.playerSO.attackValue.value * 1.3f;
                break;
            case 3:
                attackValue = PlayerController.Instance.playerSO.attackValue.value * 1.4f;
                break;
            case 4:
                attackValue = PlayerController.Instance.playerSO.attackValue.value * 1.5f;
                break;
            case 5:
                attackValue = PlayerController.Instance.playerSO.attackValue.value * 1.6f;
                break;
        }


        foreach (Collider2D hitcollider in hitColliders)
        {
            //TODO??????????????
        }
    }
    #endregion
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AttackAreaPos,attackSize);
    }
}
