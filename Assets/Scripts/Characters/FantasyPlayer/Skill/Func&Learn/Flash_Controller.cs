using UnityEngine;
using UnityEngine.UIElements;

public class Flash_Controller : MonoBehaviour
{
    private CharacterStats targetStats;
    [SerializeField] private float speed;

    private int damage;
    private Animator anim;
    private bool triggered;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Setup(int _damage,CharacterStats _targetStats)
    {
        targetStats = _targetStats;
        damage = _damage;
    }
    void Update()
    {
        if(targetStats == null) return;
        if(triggered) return;
        transform.position = Vector2.MoveTowards(transform.position,targetStats.transform.position,speed*Time.deltaTime);
        transform.up = -(targetStats.transform.position - transform.position);
        if(Vector2.Distance(transform.position,targetStats.transform.position)<0.1f)
        {
            anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(2,2);
            triggered = true;
            anim.SetTrigger("Hit");
        }
        
    }

    private void TriggerHit()
    {
        targetStats.ApplyShocked(2f);

        targetStats.TakeDamage(damage);
    }

    private void TriggerDestroy()
    {
        Destroy(gameObject);
    }
}