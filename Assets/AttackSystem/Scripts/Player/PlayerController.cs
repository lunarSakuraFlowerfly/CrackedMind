using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using InteractionSystem.Data;

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
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>(); //??????
    private Dictionary<string, Action> skillMap;
    public Vector2 lookAt = new Vector2(1, 0);
    private bool isClimb=false;
    private bool isSprint = false;
    private bool isLightAttack = false;
    private bool isHeavyAttack = false;
    //????
    private bool isAttacking = false;
    private float hitTimer=0f;
    public CharacterSO playerSO;
    public GameObject startEffectPrefab;
    //public Dictionary<PropertyType, float> characterPropertyDictionary;

    #region ???????
    [Header("???????")]
    public float jumpForce = 10f;        // ???????
    public float fallMultiplier = 2.5f;  // ??????????
    public float lowJumpMultiplier = 2f; // ????????
    private bool isGrounded;             // ??????
    public LayerMask groundLayer;        // ?????
    public float groundCheckDistance = 0.5f; // ?????????
    private bool canJump = true;         // ?????????

    [Header("???????????")]
    private bool isFalling = false;

    #endregion


    private void Start()
    {
        Application.targetFrameRate = 60; //??????60
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
        rb2D.gravityScale = 3f;  // ????????????????????§à?
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation; // ??????

        animator.SetBool("IsAttacking", false);
        //???????layer
        groundLayer = LayerMask.GetMask("Ground");
        SetupPhysics();
    }
    private void UpdateAnimator()
    {
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("Falling", isFalling);
        
    }
    private void Update()
    {
        UpdateAnimator();
        CheckGround();
        if (!isSkillTimer)
        {
            #region ????????
            float horizontal = Input.GetAxis("Horizontal");
            if (!isLightAttack && !isHeavyAttack && Input.GetKeyDown(KeyCode.LeftControl))
            {
                isSprint = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
                StartCoroutine(Sprint());
            }
            if(!isSprint && !isHeavyAttack && Input.GetKeyDown(KeyCode.J) && hitTimer <= 0.2f)
            {
                isAttacking = true;
                animator.SetBool("IsAttacking", true);
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
                    ResetAttack();
                    animator.SetBool("Attack1", false);
                    animator.SetBool("Attack2", false);
                    animator.SetBool("Attack3", false);
                }
            }
            if(!isSprint && Input.GetKeyDown(KeyCode.K))
            {
                isAttacking = true;
                animator.SetBool("IsAttacking", true);
                animator.SetFloat("MoveValue", 0);
                playerSpeed = playerSO.walkSpeed.value;
                animator.SetBool("Attack1", false);
                animator.SetBool("Attack2", false);
                animator.SetBool("Attack3", false);
                animator.SetBool("Attack4", true); //???????????????????
                isHeavyAttack = true;
            }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump && !isHeavyAttack)
            {
                Jump();
            }
            if (rb2D.velocity.y < 0)
            {
                rb2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb2D.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            if(!isHeavyAttack)
            {
                if(!Mathf.Approximately(horizontal, 0))
                {
                    lookAt.Set(Mathf.Sign(horizontal), 0);
                    animator.SetFloat("LookX", lookAt.x);
                    animator.SetFloat("LookY", 0);
                    if(isSprint || isLightAttack)
                    {
                        goto NoMove;
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        animator.SetFloat("MoveValue", 1);
                        playerSpeed = playerSO.runSpeed.value;
                    }
                    else
                    {
                        animator.SetFloat("MoveValue", 0.5f);
                        playerSpeed = playerSO.walkSpeed.value;
                    }
                    
                    rb2D.velocity = new Vector2(lookAt.x * playerSpeed, rb2D.velocity.y);
                }
                else
                {
                    rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                    animator.SetFloat("MoveValue", 0);
                }
                if (isClimb) //????
                {
                    animator.speed = animator.GetFloat("MoveValue");
                }
            }
            animator.SetBool("IsGrounded", isGrounded);
        NoMove:;
        }
        #endregion
        #region ???????
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
            playerShieldSystem.ChangeShieldValue(-10);
            
        }
        #endregion

        // ??????????????????????
        if (!isGrounded && rb2D.velocity.y < 0)
        {
            isFalling = true;
        }
        
        // ???????????
        UpdateJumpAnimation();
    }

    /// <summary>
    /// ?????????????
    /// </summary>
    private void UpdateJumpAnimation()
    {
        animator.SetBool("Falling", isFalling);
        animator.SetBool("IsGrounded", isGrounded);

        // ?????????????????
        if (isGrounded && isFalling)
        {
            isFalling = false;
        }
    }

    public void HeavyAttackFalse()
    {
        isHeavyAttack = false;
        isAttacking = false;
        animator.SetBool("Attack4", false);
        animator.SetBool("IsAttacking", false);
        Debug.Log("???????");
    }
    public void LightAttackMove(float moveDis)
    {
        rb2D.velocity = new Vector2(lookAt.x * moveDis, rb2D.velocity.y);
    }
    public void HeavyAttackMove(float moveDis)
    {
        rb2D.velocity = new Vector2(lookAt.x * moveDis, rb2D.velocity.y);
    }
    IEnumerator Sprint()
    {
        float timer = 0f;
        while(timer < 0.3f)
        {
            rb2D.velocity = new Vector2(lookAt.x * sprintSpeed, rb2D.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }
        isSprint = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
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

    public void UseItem(ItemData itemData)
    {
        foreach(var property in itemData.propertyList)
        {
            PropertyChange(property.propertyType, property.value);
        }
    }

    public void TakeDamage(float damageValue)
    {
        PropertyChange(PropertyType.HpValue, damageValue);
        playerSO.soberValue.value += playerSO.soberChangeSpeed.value;
        if(playerSO.soberValue.value > 100)
        {
            //TODO:??????
        }
    }

    private void Died()
    {
        //TODO:???? 
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
                Debug.Log("??????");
                break;
            //case PropertyType.MagicValue:
            //    playerSO.currentMagic.value =Math.Clamp(playerSO.currentMagic.value + changeValue, 0, playerSO.maxMagic.value);
            //    Debug.Log("???????");
            //    break;
            //case PropertyType.HungryValue:
            //    playerSO.currentHungry.value = Math.Clamp(playerSO.currentHungry.value + changeValue, 0, playerSO.maxHungry.value);
            //    Debug.Log("???????");
            //    break;
            case PropertyType.Exp:
                StartCoroutine(PlayerLevelUp(changeValue));
                PlayerUI.Instance.UpdatePlayerPropertyUI();
                Debug.Log("????????");
                break;
            case PropertyType.AttackValue:
                playerSO.attackValue.value += changeValue;
                Debug.Log("?????????");
                break;
            case PropertyType.AttackSpeed:
                playerSO.attackSpeed.value += changeValue;
                Debug.Log("?????????");
                break;
            case PropertyType.MoveSpeed:
                playerSO.walkSpeed.value += changeValue;
                Debug.Log("????????");
                break;
            //case PropertyType.Lucky:
            //    playerSO.criticalChance.value += changeValue;
            //    Debug.Log("????????");
            //    break;
            //case PropertyType.CriticalEffect:
            //    playerSO.criticalEffect.value += changeValue;
            //    Debug.Log("????§¹?????");
            //    break;
            case PropertyType.DefensiveValue:
                playerSO.defensiveValue.value += changeValue;
                Debug.Log("?????????");
                break;
            default:
                Debug.Log("?????????");
                break;
        }
          
    }

    IEnumerator PlayerLevelUp(float expValue)
    {
        playerSO.exp.value += expValue;
        float expNeed = 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value)); //???????Þo??? 20+x^(2.25+log(x))
        while(playerSO.exp.value >= expNeed)
        {
            Debug.Log("????");
            playerSO.exp.value -= expNeed;
            playerSO.level.value += 1;
            expNeed = 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value));
            yield return null;
        }
    }


    #region Skill
    private void UpdateSkillMap()
    {
        skillMap = new Dictionary<string, Action> //????????????
        {
            { "HeartSlash",HeartSlash},
            { "MindShield",MindShield},
        };
    }
    private void UseSkill(string skillName)
    {
        if(!IsSkillCoolDown(skillName))//????cd??
        {
            if(skillMap.TryGetValue(skillName, out Action action)) //??????????????????action
            {
                action.Invoke();
                
            }
        }
        else
        {
            Debug.Log($"{skillName}???????cd??");
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

        cooldowns.Add(skillName, coolTime+Time.time); //????????Time.time
        Debug.Log("???????cd?????");
    }
    private void ChangeIsSkillTimer()
    {
        isSkillTimer = !isSkillTimer;
        Debug.Log("???IsSkillTimer" + PlayerController.Instance.isSkillTimer);
    }
    public void HeartSlash() //???
    {
        if (playerSkillSO.HeartSlash.skillLevel == 0)
        {
            Debug.Log("????HeartSlash¦Ä????");
            return;
        }
        //if()
        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("HeartSlash");
        Debug.Log("??????HeartSlash");
        ChangeIsSkillTimer();
        Invoke(nameof(ChangeIsSkillTimer), 0.35f);
        if (cooldowns.ContainsKey("HeartSlash"))
            cooldowns.Remove("HeartSlash");
        AddSkillCoolDown("HeartSlash", playerSkillSO.HeartSlash.skillTime);
        //???????????????
        Invoke(nameof(ResetAttack), 0.35f);
    }
    public void MindShield()
    {
        if(playerSkillSO.MindShield.skillLevel == 0)
        {
            Debug.Log("????MindShield¦Ä????");
            return;
        }
        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("MindShield");
        Debug.Log("??????MindShield");
        ChangeIsSkillTimer();
        Invoke(nameof(ChangeIsSkillTimer), 0.35f);
        if (cooldowns.ContainsKey("MindShield"))
            cooldowns.Remove("MindShield");
        AddSkillCoolDown("MindShield", playerSkillSO.MindShield.skillTime);
        //???????????????
        Invoke(nameof(ResetAttack), 0.35f);
    }
    #endregion

    /// <summary>
    /// ??????
    /// </summary>
    private void Jump()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
        animator.SetTrigger("Jump");  // ????????????
        isGrounded = false;
        canJump = false;
        StartCoroutine(ResetJump());
    }

    /// <summary>
    /// ?????????
    /// </summary>
    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }

    /// <summary>
    /// ???????????????
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ??????????????
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            if (isFalling)
            {
                isFalling = false;
                // ???????????Animator??????????????
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    private void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    /// <summary>
    /// ??????Layer??????????
    /// </summary>
    private void CheckGround()
    {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        
        // ????????????????
        //Debug.Log($"???????: {transform.position}, ??????: {groundCheckDistance}");
        
        isGrounded = hit.collider != null;
        /*
        if (hit.collider != null)
        {
            Debug.Log($"??????—¨????: {hit.distance}, ????????: {hit.collider.gameObject.name}");
        }
        else
        {
            Debug.Log("¦Ä???????");
        }
        */
    }

    /// <summary>
    /// ???????????????????
    /// </summary>
    private void SetupPhysics()
    {
        // 1. ???????????????????
        PhysicsMaterial2D slipperyMaterial = new PhysicsMaterial2D("Slippery");
        slipperyMaterial.friction = 0f;      // ??????????0
        slipperyMaterial.bounciness = 0f;    // ????????0

        // 2. ????????????
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.sharedMaterial = slipperyMaterial;
        }

        // 3. ???????????
        if (rb2D != null)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;  // ???????
            rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // ?????????????
        }
    }
}
