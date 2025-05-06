using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : Singleton<PlayerController>
{
    private float playerSpeed;
    public float sprintSpeed;
    private Collider2D cd2D;
    public Rigidbody2D rb2D;
    private Animator animator;
    private SkillTree playerSkillTree;
    public SkillSO playerSkillSO;
    public Shield playerShieldSystem;
    public bool isSkillTimer = false;
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>(); //记录冷却
    private Dictionary<string, Action> skillMap;
    public Vector2 lookAt = new Vector2(0, -1);
    private bool isClimb=false;
    private bool isSprint = false;
    private bool isLightAttack = false;
    private bool isHeavyAttack = false;
    private float hitTimer=0f;
    public CharacterSO playerSO;
    public GameObject startEffectPrefab;
    //public Dictionary<PropertyType, float> characterPropertyDictionary;

    private void Start()
    {
        Application.targetFrameRate = 60; //帧率设为60
        rb2D = GetComponent<Rigidbody2D>();
        cd2D = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        playerSkillTree = GetComponent<SkillTree>();    
        playerSO.currentHp.value = playerSO.maxHp.value;
        //playerSO.currentMagic.value = playerSO.maxMagic.value;
        //playerSO.currentHungry.value = playerSO.maxHungry.value;
        playerSpeed = playerSO.walkSpeed.value;
        PlayerUI.Instance.UpdatePlayerPropertyUI();
        //characterPropertyDictionary = new Dictionary<PropertyType, float>();
        //characterPropertyDictionary.Add(characterSO.propertyList[0].propertyType, characterSO.propertyList[0].value);
        UpdateSkillMap();
    }
    private void Update()
    {
        if (!isSkillTimer)
        {

            #region 基本操作
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (!isLightAttack && !isHeavyAttack && Input.GetKeyDown(KeyCode.LeftControl))
            {
                isSprint = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true); //禁用碰撞
            
                StartCoroutine(Sprint());
            }
            if(!isSprint && !isHeavyAttack && Input.GetKeyDown(KeyCode.J) && hitTimer <= 0.2f)
            {
                animator.SetFloat("AttackSpeed",playerSO.attackSpeed.value);
                hitTimer = 1.0f/playerSO.attackSpeed.value + 0.2f;
                isLightAttack = true;
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
            if (isLightAttack)
            {
                hitTimer -= Time.deltaTime;
                if(hitTimer < 0)
                {
                    isLightAttack = false;
                    animator.SetBool("Attack1", false);
                    animator.SetBool("Attack2", false);
                    animator.SetBool("Attack3", false);
                }
            }
            if(!isSprint && Input.GetKeyDown(KeyCode.K))
            {
                animator.SetFloat("MoveValue", 0);
                playerSpeed = playerSO.walkSpeed.value;
                animator.SetBool("Attack1", false);
                animator.SetBool("Attack2", false);
                animator.SetBool("Attack3", false);
                animator.SetBool("Attack4", true); //动画结束后执行取消重击
                isHeavyAttack = true;
            }
            if(!isHeavyAttack)
                if(!Mathf.Approximately(horizontal,0)|| !Mathf.Approximately(vertical, 0)) //其中一个不为0
                {
                    lookAt.Set(horizontal, vertical);
                    lookAt.Normalize();
                    animator.SetFloat("LookX",lookAt.x);
                    animator.SetFloat("LookY",lookAt.y);
                    if(isSprint ||  isLightAttack)
                    {
                        goto NoMove; //如果处于非移动状态，只改变方向，不移动
                    }
                    if (Input.GetKey(KeyCode.LeftShift)) //注意不是getkeydown
                    {
                        animator.SetFloat("MoveValue", 1); //奔跑
                        playerSpeed = playerSO.runSpeed.value;
                    }
                    else
                    {
                        animator.SetFloat("MoveValue", 0.5f); //行走
                        playerSpeed = playerSO.walkSpeed.value;
                    }
                    rb2D.MovePosition((Vector2)transform.position+lookAt*playerSpeed*Time.deltaTime);
                }
                else
                {
                    animator.SetFloat("MoveValue", 0);
                }
                if (isClimb) //攀爬
                {
                    animator.speed = animator.GetFloat("MoveValue");
                }
        NoMove:;
            //transform.Translate(playerSpeed*horizontal * Time.deltaTime, playerSpeed*vertical * Time.deltaTime,0,Space.Self);
            //Vector2 position = new Vector2(playerSpeed * horizontal * Time.deltaTime, playerSpeed * vertical * Time.deltaTime);
        }
        #endregion
        #region 技能操作
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill("HeartSlash");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill("MindShield");
        }
        if(Input.GetKeyDown(KeyCode.Minus))
        {
            playerShieldSystem.ChangeShieldValue(10);
            
        }
        #endregion
    }

    public void HeavyAttackFalse()
    {
        isHeavyAttack = false;
        animator.SetBool("Attack4", false);
    }
    public void LightAttackMove(float moveDis)
    {
        rb2D.MovePosition((Vector2)transform.position+lookAt*moveDis*Time.deltaTime);
    }
    public void HeavyAttackMove(float moveDis)
    {
        rb2D.MovePosition((Vector2)transform.position+lookAt*moveDis*Time.deltaTime);
    }
    IEnumerator Sprint()
    {
        float timer = 0f;
        while(timer < 0.3f) //小于冲刺时间0.3
        {
            rb2D.MovePosition((Vector2)transform.position + lookAt * sprintSpeed * Time.deltaTime); //冲刺速度为10
            timer += Time.deltaTime;
            yield return null;
        }
        isSprint = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false); //启用碰撞
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

    public void TakeDamage(float damageValue)
    {
        PropertyChange(PropertyType.HpValue, damageValue);
        playerSO.soberValue.value += playerSO.soberChangeSpeed.value;
        if(playerSO.soberValue.value > 100)
        {
            //TODO:回到现实
        }
    }

    private void Died()
    {
        //TODO:死亡 
    }
    public void PropertyChange(PropertyType changePropertyType, float changeValue)
    {
      
        switch (changePropertyType)
        {
            case PropertyType.HpValue:
                playerSO.currentHp.value =Mathf.Clamp(playerSO.currentHp.value + changeValue, 0, playerSO.maxHp.value);
                PlayerUI.Instance.UpdatePlayerPropertyUI();
                if (Mathf.Approximately(playerSO.currentHp.value, 0))
                {
                    Died();
                }
                Debug.Log("血量改变");
                break;
            //case PropertyType.MagicValue:
            //    playerSO.currentMagic.value =Math.Clamp(playerSO.currentMagic.value + changeValue, 0, playerSO.maxMagic.value);
            //    Debug.Log("蓝量改变");
            //    break;
            //case PropertyType.HungryValue:
            //    playerSO.currentHungry.value = Math.Clamp(playerSO.currentHungry.value + changeValue, 0, playerSO.maxHungry.value);
            //    Debug.Log("饱食度改变");
            //    break;
            case PropertyType.Exp:
                StartCoroutine(PlayerLevelUp(changeValue));
                PlayerUI.Instance.UpdatePlayerPropertyUI();
                Debug.Log("经验值改变");
                break;
            case PropertyType.AttackValue:
                playerSO.attackValue.value += changeValue;
                Debug.Log("攻击力改变");
                break;
            case PropertyType.AttackSpeed:
                playerSO.attackSpeed.value += changeValue;
                Debug.Log("攻击速度改变");
                break;
            case PropertyType.MoveSpeed:
                playerSO.walkSpeed.value += changeValue;
                Debug.Log("移动速度改变");
                break;
            //case PropertyType.Lucky:
            //    playerSO.criticalChance.value += changeValue;
            //    Debug.Log("暴击率改变");
            //    break;
            //case PropertyType.CriticalEffect:
            //    playerSO.criticalEffect.value += changeValue;
            //    Debug.Log("暴击效果改变");
            //    break;
            case PropertyType.DefensiveValue:
                playerSO.defensiveValue.value += changeValue;
                Debug.Log("防御力改变");
                break;
            default:
                Debug.Log("其他改变改变");
                break;
        }
          
    }

    IEnumerator PlayerLevelUp(float expValue)
    {
        playerSO.exp.value += expValue;
        float expNeed = 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value)); //升级所需经验值 20+x^(2.25+log(x))
        while(playerSO.exp.value >= expNeed)
        {
            Debug.Log("升级");
            playerSO.exp.value -= expNeed;
            playerSO.level.value += 1;
            expNeed = 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value));
            yield return null;
        }
    }


    #region Skill
    private void UpdateSkillMap()
    {
        skillMap = new Dictionary<string, Action> //初始化技能字典
        {
            { "HeartSlash",HeartSlash},
            { "MindShield",MindShield},
        };
    }
    private void UseSkill(string skillName)
    {
        if(!IsSkillCoolDown(skillName))//不在cd中
        {
            if(skillMap.TryGetValue(skillName, out Action action)) //尝试获取函数名并传给action
            {
                action.Invoke();
                
            }
        }
        else
        {
            Debug.Log($"{skillName}技能还在cd中");
        }
    }
    
    private bool IsSkillCoolDown(string skillName)
    {
        return cooldowns.ContainsKey(skillName) && cooldowns[skillName] > Time.time;
    }
    private void AddSkillCoolDown(string skillName, float coolTime)
    {
        if (skillMap[skillName] == null)
            return;

        cooldowns.Add(skillName, coolTime+Time.time); //注意要加上Time.time
        Debug.Log("添加技能cd到字典");
    }
    private void ChangeIsSkillTimer()
    {
        isSkillTimer = !isSkillTimer;
        Debug.Log("改变IsSkillTimer" + PlayerController.Instance.isSkillTimer);
    }
    public void HeartSlash() //模版
    {
        if (playerSkillSO.HeartSlash.skillLevel == 0)
        {
            Debug.Log("技能HeartSlash未解锁");
            return;
        }
        //if()
        animator.SetTrigger("HeartSlash");
        Debug.Log("释放技能HeartSlash");
        ChangeIsSkillTimer();
        Invoke(nameof(ChangeIsSkillTimer), 0.35f);
        if (cooldowns.ContainsKey("HeartSlash"))
            cooldowns.Remove("HeartSlash");
        AddSkillCoolDown("HeartSlash", playerSkillSO.HeartSlash.skillTime);
    }
    public void MindShield()
    {
        if(playerSkillSO.MindShield.skillLevel == 0)
        {
            Debug.Log("技能MindShield未解锁");
            return;
        }
        animator.SetTrigger("MindShield");
        Debug.Log("释放技能MindShield");
        ChangeIsSkillTimer();
        Invoke(nameof(ChangeIsSkillTimer), 0.35f);
        if (cooldowns.ContainsKey("MindShield"))
            cooldowns.Remove("MindShield");
        AddSkillCoolDown("MindShield", playerSkillSO.MindShield.skillTime);

    }
    #endregion
}
