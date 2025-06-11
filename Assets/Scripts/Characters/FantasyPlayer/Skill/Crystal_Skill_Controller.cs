using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private float crystalExitTimer;
    private Animator anim =>GetComponent<Animator>();
    private CircleCollider2D cd =>GetComponent<CircleCollider2D>();
    private bool canExplode;
    private bool canMove;
    private bool canGrow;
    private float moveSpeed;
    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    [SerializeField] private float growSpeed;
    public void SetupCrystal(float _crystalDuration,bool _canExplode,bool _canMove,float _moveSpeed,Transform _closestTarget)
    {
        crystalExitTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }
    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackholeSkill.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
        if(colliders.Length > 0)
        {
            closestTarget = colliders[Random.Range(0,colliders.Length)].transform;
        }
        
    }
    private void Update()
    {
        crystalExitTimer -= Time.deltaTime;
        if(crystalExitTimer < 0)
        {
            FinishCrystal();
        }

        if(canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position,closestTarget.position,moveSpeed*Time.deltaTime);
            if(Vector2.Distance(transform.position,closestTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if(canGrow)
        {
            transform.localScale = Vector3.Lerp(transform.localScale,new Vector2(3,3),growSpeed*Time.deltaTime);
        }
    }
    public void FinishCrystal()
    {
        if(canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerManager.instance.player.Stats.DoDamage(enemy.GetComponent<CharacterStats>());
                ItemData_Equipment equipedAmulet =  Inventory.instance.GetEquipmentByType(EquipmentType.Amulet);
                if(equipedAmulet != null)
                {
                    equipedAmulet.ExecuteItemEffect(enemy.transform);
                }
            }
        }
    }

    public void SelfDestroy()=>Destroy(gameObject);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cd.radius);
    }
}
