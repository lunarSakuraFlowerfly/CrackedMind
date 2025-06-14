
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed=12.0f;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    [Header("Pierce info")]
    [SerializeField] private int amountOfPierce;
    [SerializeField] private float pierceSpeed;
    private bool isPierce;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;
    private float hitTimer;
    private float hitCooldown;
    private float spinDirection;

    [Header("Bounce info")]
    [SerializeField]private float bounceSpeed = 20;
    private bool isBouncing;
    private int amountOfBounce;
    private List<Transform> enemyTarget;
    private int targetIndex;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }
    public void SetupSword(Vector2 _dir,float _gravityScale,Player _player,float _freezeTimeDuration,float _returnSpeed)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        anim.SetBool("Rotation",true);
        spinDirection = Mathf.Clamp(rb.velocity.x,-1,1);
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;
        Invoke("DestroyMe",7);
    }

    public void SetupBounce(bool _isBouncing,int _amountOfBounce,float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        amountOfBounce = _amountOfBounce;
        bounceSpeed = _bounceSpeed;
        enemyTarget = new List<Transform>();
    }
    public void SetupPierce(bool _isPierce,int _pierceAmount)
    {
        isPierce = _isPierce;
        amountOfPierce = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning,float _maxTravelDistance,float _spinDuration,float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }
    public void ReturnSword()
    {
        //rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;

    }
    private void Update()
    {
        if(canRotate)
            transform.right = rb.velocity;

        if(isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position,player.transform.position,Time.deltaTime*returnSpeed);
            if(Vector2.Distance(transform.position,player.transform.position)<0.2)
            {
                player.ClearTheSword();
            }
        }
        BounceLogic();
        SpinLogic();

    }
    private void SpinLogic()
    {
        if(isSpinning)
        {
            if(Vector2.Distance(transform.position,player.transform.position)>maxTravelDistance&&!wasStopped)
            {
                stopWhenSpinning();
            }
            if(wasStopped)
            {
                spinTimer -= Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x+spinDirection,transform.position.y),1.5f*Time.deltaTime);
                if(spinTimer<=0)
                {
                    isSpinning = false;
                    isReturning = true;
                }
                hitTimer -= Time.deltaTime;
                if(hitTimer<0)
                {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,1);
                    foreach(Collider2D collider in colliders)
                    {
                        if(collider.GetComponent<Enemy>()!=null)
                        {
                            SwordSkillDamage(collider.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }
    private void stopWhenSpinning()
    {
        Debug.Log("stopWhenSpinning");
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }
    private void BounceLogic()
    {
        if(isBouncing && enemyTarget.Count>0)
        {
            transform.position = Vector2.MoveTowards(transform.position,enemyTarget[targetIndex].position,Time.deltaTime*bounceSpeed);
            if(Vector2.Distance(transform.position,enemyTarget[targetIndex].position)<0.1f)
            {
                if(enemyTarget[targetIndex].GetComponent<Enemy>()!=null)
                {
                    SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                }
                targetIndex++;
                amountOfBounce--;

                if(amountOfBounce<=0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                if(targetIndex>=enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if(isReturning)
            return;

        if(collision.GetComponent<Enemy>()!=null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }
        SetupTargetsForBounce(collision);
        StuckInto(collision);
    }
    private void SwordSkillDamage(Enemy enemy)
    {
        PlayerManager.instance.player.Stats.DoDamage(enemy.GetComponent<CharacterStats>());
        enemy.StartCoroutine("FreezeTimeFor",freezeTimeDuration);
    }
    private void SetupTargetsForBounce(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!=null)
        {
            if(isBouncing && enemyTarget.Count<=0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,10);
                foreach(Collider2D collider in colliders)
                {
                    if(collider.GetComponent<Enemy>()!=null)
                    {
                        enemyTarget.Add(collider.transform);
                    }
                }
            }
        }
    }
    private void StuckInto(Collider2D collision)
    {
        if(isPierce && amountOfPierce>0&&collision.GetComponent<Enemy>()!=null)
        {
            amountOfPierce--;
            return;
        }
        if(isSpinning)
        {
            
            stopWhenSpinning();
            return;
        }
        canRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if(isBouncing && enemyTarget.Count>0)
            return;
        anim.SetBool("Rotation",false);
        transform.parent = collision.transform;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
