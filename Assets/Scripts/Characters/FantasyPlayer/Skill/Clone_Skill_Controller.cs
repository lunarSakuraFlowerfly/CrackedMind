using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;
    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius=.8f;
    private Transform closestEnemy;
    private bool canDuplicateClone;
    private int facingDir;
    private float chanceToDuplicateClone;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {

        cloneTimer -= Time.deltaTime;
        if(cloneTimer<0)
        {
            sr.color = new Color(1,1,1,sr.color.a - colorLoosingSpeed * Time.deltaTime);
            if(sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack,Vector3 _offset,Transform _closestEnemy,bool _canDuplicateClone,float _chanceToDuplicateClone)
    {
        if (_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 3));
            
        }
        transform.position = _newTransform.position+_offset;
        cloneTimer = _cloneDuration;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicateClone = _chanceToDuplicateClone;
        FaceClosestEnemy();
    }


    private void AnimationTrigger()=>cloneTimer = -.1f;

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerManager.instance.player.Stats.DoDamage(enemy.GetComponent<CharacterStats>());
                if(canDuplicateClone)
                {
                    if(Random.Range(0,100)<chanceToDuplicateClone)
                    {
                        SkillManager.instance.cloneSkill.CreateClone(collider.transform,new Vector3(.5f*facingDir,0));
                        canDuplicateClone = false;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    private void FaceClosestEnemy()
    {

        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0,180,0);
            }
        }
        
        
    }
}
