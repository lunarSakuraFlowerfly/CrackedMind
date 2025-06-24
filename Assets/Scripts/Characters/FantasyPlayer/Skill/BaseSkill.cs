using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;



public class BaseSkill : MonoBehaviour
{
    [Header("技能类型")]
    [SerializeField] protected SkillType skillType=SkillType.Assignable;
    public SkillType Skill_Type => skillType;



    public float cooldown;
    protected float cooldownTimer;
    public float CooldownTimer => cooldownTimer;
    public bool IsOnCooldown => cooldownTimer > 0;

    //冷却时间占比
    public float cooldownProgress;
    public float CooldownProgress => cooldownProgress;


    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
        CheckUnlock();
    }
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
        cooldownProgress = cooldownTimer / cooldown;
    }
    public virtual bool CanUseSkill()
    {
        if(cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }
        return false;
    }
    public virtual void UseSkill()
    {
        Debug.Log("使用技能");
    }
    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25f);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                float distance = Vector2.Distance(_checkTransform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        return closestEnemy;
    }

    protected virtual void CheckUnlock()
    {
        
    }
}
