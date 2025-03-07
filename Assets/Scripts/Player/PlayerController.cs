using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    private float playerSpeed;
    public float sprintSpeed;
    private Collider2D cd2D;
    private Rigidbody2D rb2D;
    private Animator animator;
    private Vector2 lookAt = new Vector2(-1, 0);
    private bool isClimb=false;
    private bool isSprint = false;
    private bool isAttack = false;
    private float hitTimer=0f;
    public CharacterSO playerSO;
    public GameObject startEffectPrefab;
    //public Dictionary<PropertyType, float> characterPropertyDictionary;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60; //֡����Ϊ60
        rb2D = GetComponent<Rigidbody2D>();
        cd2D = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        playerSO.currentHp.value = playerSO.maxHp.value;
        //playerSO.currentMagic.value = playerSO.maxMagic.value;
        //playerSO.currentHungry.value = playerSO.maxHungry.value;
        playerSpeed = playerSO.walkSpeed.value;
        PlayerUI.Instance.UpdatePlayerPropertyUI();
        //characterPropertyDictionary = new Dictionary<PropertyType, float>();
        //characterPropertyDictionary.Add(characterSO.propertyList[0].propertyType, characterSO.propertyList[0].value);
    }
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (!isAttack && Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprint = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true); //������ײ
            
            StartCoroutine(Sprint());
        }
        if(!isSprint && Input.GetKeyDown(KeyCode.J) && hitTimer <= 0.2f)
        {
            animator.SetFloat("AttackSpeed",playerSO.attackSpeed.value);
            hitTimer = 1.0f/playerSO.attackSpeed.value + 0.2f;
            isAttack = true;
            if (!animator.GetBool("Attack1"))
            {
                animator.SetBool("Attack1", true);
            }else if (!animator.GetBool("Attack2"))
            {
                animator.SetBool("Attack2", true);
            }else if (!animator.GetBool("Attack3"))
            {
                animator.SetBool("Attack3", true);
            }else if (animator.GetBool("Attack3"))
            {
                animator.SetBool("Attack1", true);
                animator.SetBool("Attack2", false);
                animator.SetBool("Attack3", false);
            }
        }
        if (isAttack)
        {
            hitTimer -= Time.deltaTime;
            if(hitTimer < 0)
            {
                isAttack = false;
                animator.SetBool("Attack1", false);
                animator.SetBool("Attack2", false);
                animator.SetBool("Attack3", false);
            }
        }
        if (!isSprint && !isAttack)
        {
            if(!Mathf.Approximately(horizontal,0)|| !Mathf.Approximately(vertical, 0)) //����һ����Ϊ0
            {
                lookAt.Set(horizontal, vertical);
                lookAt.Normalize();
                animator.SetFloat("LookX",lookAt.x);
                animator.SetFloat("LookY",lookAt.y);
                if (Input.GetKey(KeyCode.LeftShift)) //ע�ⲻ��getkeydown
                {
                    animator.SetFloat("MoveValue", 1); //����
                    playerSpeed = playerSO.runSpeed.value;
                }
                else
                {
                    animator.SetFloat("MoveValue", 0.5f); //����
                    playerSpeed = playerSO.walkSpeed.value;
                }
                rb2D.MovePosition((Vector2)transform.position+lookAt*playerSpeed*Time.deltaTime);
            }
            else
            {
                animator.SetFloat("MoveValue", 0);
            }
            if (isClimb) //����
            {
                animator.speed = animator.GetFloat("MoveValue");
            }
        }
        //transform.Translate(playerSpeed*horizontal * Time.deltaTime, playerSpeed*vertical * Time.deltaTime,0,Space.Self);
        //Vector2 position = new Vector2(playerSpeed * horizontal * Time.deltaTime, playerSpeed * vertical * Time.deltaTime);
    }

    IEnumerator Sprint()
    {
        float timer = 0f;
        while(timer < 0.3f) //С�ڳ��ʱ��0.3
        {
            rb2D.MovePosition((Vector2)transform.position + lookAt * sprintSpeed * Time.deltaTime); //����ٶ�Ϊ10
            timer += Time.deltaTime;
            yield return null;
        }
        isSprint = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false); //������ײ
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tag.PICKUPABLIE))
        {
            PickableObject po = collision.gameObject.GetComponent<PickableObject>();
            po?.Pickup();
            Instantiate(startEffectPrefab);
        }
        if (collision.CompareTag(Tag.CLIMBABLE))
        {
            animator.SetBool("Climb", true);
            animator.speed = 0;
            isClimb = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tag.CLIMBABLE))
        {
            animator.SetBool("Climb", false);
            animator.speed = 1;
            isClimb = false;
        }
    }

    public void UseItem(ItemSO itemSO)
    {
        foreach(var property in itemSO.propertyList)
        {
            PropertyChange(property.propertyType,property.value);
        }
    }

    public void PropertyChange(PropertyType changePropertyType, float changeValue)
    {
      
        switch (changePropertyType)
        {
            case PropertyType.HpValue:
                playerSO.currentHp.value =Mathf.Clamp(playerSO.currentHp.value + changeValue, 0, playerSO.maxHp.value);
                PlayerUI.Instance.UpdatePlayerPropertyUI();
                Debug.Log("Ѫ���ı�");
                break;
            //case PropertyType.MagicValue:
            //    playerSO.currentMagic.value =Math.Clamp(playerSO.currentMagic.value + changeValue, 0, playerSO.maxMagic.value);
            //    Debug.Log("�����ı�");
            //    break;
            //case PropertyType.HungryValue:
            //    playerSO.currentHungry.value = Math.Clamp(playerSO.currentHungry.value + changeValue, 0, playerSO.maxHungry.value);
            //    Debug.Log("��ʳ�ȸı�");
            //    break;
            case PropertyType.Exp:
                StartCoroutine(PlayerLevelUp(changeValue));
                PlayerUI.Instance.UpdatePlayerPropertyUI();
                Debug.Log("����ֵ�ı�");
                break;
            case PropertyType.AttackValue:
                playerSO.attackValue.value += changeValue;
                Debug.Log("�������ı�");
                break;
            case PropertyType.AttackSpeed:
                playerSO.attackSpeed.value += changeValue;
                Debug.Log("�����ٶȸı�");
                break;
            case PropertyType.MoveSpeed:
                playerSO.walkSpeed.value += changeValue;
                Debug.Log("�ƶ��ٶȸı�");
                break;
            //case PropertyType.Lucky:
            //    playerSO.criticalChance.value += changeValue;
            //    Debug.Log("�����ʸı�");
            //    break;
            //case PropertyType.CriticalEffect:
            //    playerSO.criticalEffect.value += changeValue;
            //    Debug.Log("����Ч���ı�");
            //    break;
            case PropertyType.DefensiveValue:
                playerSO.defensiveValue.value += changeValue;
                Debug.Log("�������ı�");
                break;
            default:
                Debug.Log("�����ı�ı�");
                break;
        }
          
    }

    IEnumerator PlayerLevelUp(float expValue)
    {
        playerSO.exp.value += expValue;
        float expNeed = 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value)); //�������辭��ֵ 20+x^(2.25+log(x))
        while(playerSO.exp.value >= expNeed)
        {
            Debug.Log("����");
            playerSO.exp.value -= expNeed;
            playerSO.level.value += 1;
            expNeed = 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value));
            yield return null;
        }
    }
}
