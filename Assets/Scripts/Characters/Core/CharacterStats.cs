using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;
public enum StatType
{
    Strength,
    Agility,
    Intelligence,
    Vitality,
    Damage,
    CritChance,
    CritPower,
    Health,
    Armor,
    MagicResistance,
    Evasion,
    FireDamage,
    IceDamage,
    LightningDamage,

}
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;
    [Header("Major Stats")]
    public Stat strength;//力量
    public Stat agility;//敏捷
    public Stat intelligence;//智力
    public Stat vitality;//体力

    [Header("Defensive Stats")]
    public Stat maxHP;//最大生命值
    public Stat armor;//护甲
    public Stat magicResistance;//魔法抗性
    public Stat evasion;//闪避


    [Header("Offensive Stats")]
    public Stat damage;//伤害
    public Stat critChance;//暴击率
    public Stat critPower;//暴击伤害
    [Header("运动")]
    public Stat attackSpeedPercentage;//攻击速度
    public Stat moveSpeedPercentage;//移动速度
    public Stat jumpForcePercentage;//跳跃力
    public Stat dashSpeedPercentage;//冲刺速度


    [Header("Magic Stats")]
    public Stat fireDamage;//火属性伤害
    public Stat iceDamage;//冰属性伤害
    public Stat lightningDamage;//雷属性伤害

    public bool isIgnited;//是否点燃 持续造成伤害
    public bool isFrozend;//是否冰冻 减少20%防御
    public bool isShocked;//是否电击 降低命中率

    [Header("Shield Stats")]
    [SerializeField] private int shieldValue;


    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown =.3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    public int shockDamage;
    public bool isDead;

    

    [SerializeField] private int currentHP;//当前生命值
    public System.Action OnHealthChanged;

    public bool isInvincible{get;private set;}

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;
        if(ignitedTimer<0)
            isIgnited = false;
        if(chilledTimer<0)
            isFrozend = false;
        if(shockedTimer<0)
            isShocked = false;
        ApplyIgniteDamage();

    }
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHP = GetMaxHP();
        fx = GetComponentInChildren<EntityFX>();

    }

    /// <summary>
    /// 受伤计算
    /// </summary>
    /// <param name="_damage"></param>
    public virtual void TakeDamage(int _damage)
    {
        if(_damage<=0) return;
        DecreaseHealthBy(_damage);
        GetComponent<Entity>().DamageFX();
        if(currentHP <= 0)
        {
            Die();
        }
    }
    public virtual void TakeDamage(int _damage,float _force,Vector2 _direction,float _duration)
    {
        if(isInvincible) return;
        if(_damage<=0) return;
        DecreaseHealthBy(_damage);
        GetComponent<Entity>().DamageFX(_force,_direction,_duration);
        if(currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        if(shieldValue>0)
        {
            shieldValue -= _damage;
            if(shieldValue<0)
            {
                currentHP += shieldValue;
                shieldValue = 0;
            }
            
        }
        else
        {
            if(_damage>GetMaxHP()*0.3f)
            {
                GetComponent<Entity>().SetupKnockbackPower(new Vector2(10,15));
            }
            currentHP -= _damage;
        }
        if(OnHealthChanged!=null)
        {
            OnHealthChanged.Invoke();
        }
    }

    #region 伤害计算

    /// <summary>
    /// 伤害总计算
    /// </summary>
    /// <param name="_targetStats"></param>
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if(_targetStats.currentHP<=0) return;
        _targetStats.GetComponent<Entity>().SetKnockbackDir(transform);
        DoPhysicalDamage(_targetStats);
        DoMagicDamage(_targetStats);
    }
    

    /// <summary>
    /// 物理伤害计算
    /// </summary>
    /// <param name="_targetStats"></param>
    public virtual void DoPhysicalDamage(CharacterStats _targetStats)
    {
        if(canEvasion(_targetStats)) return;
        int totalDamage = PhysicalDamageCalculate(_targetStats);
        _targetStats.TakeDamage(totalDamage);

    }

    /// <summary>
    /// 物理伤害计算逻辑
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <returns></returns>
    private int PhysicalDamageCalculate(CharacterStats _targetStats)
    {
        float totalDamage = damage.GetValue() + strength.GetValue();
        if(isFrozend)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue()*0.8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }
        totalDamage = Mathf.Clamp(totalDamage,0,int.MaxValue);        
        if(CanCrit())
        {
            totalDamage *= critPower.GetValue()/100.0f;
        }
        return Mathf.RoundToInt(totalDamage);
    }

    /// <summary>
    /// 魔法伤害计算
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <returns></returns>

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();
        int _totalDamage = _fireDamage+_iceDamage+_lightningDamage + intelligence.GetValue();
        _totalDamage -= _targetStats.magicResistance.GetValue()+_targetStats.intelligence.GetValue()*3;
        _totalDamage = Mathf.Clamp(_totalDamage,0,int.MaxValue);
        _targetStats.TakeDamage(_totalDamage);

        if(Mathf.Max(_fireDamage,_iceDamage,_lightningDamage)<=0) return;
        
        AttemptToApplyAilments(_fireDamage,_iceDamage,_lightningDamage,_targetStats);
   
    }
    protected void AttemptToApplyAilments(int _fireDamage,int _iceDamage,int _lightningDamage,CharacterStats _targetStats)
    {
        bool canApplyIgnite = _fireDamage>_iceDamage&&_fireDamage>_lightningDamage;
        bool canApplyFrozen = _iceDamage>_fireDamage&&_iceDamage>_lightningDamage;
        bool canApplyShocked = _lightningDamage>_fireDamage&&_lightningDamage>_iceDamage;
        if(!canApplyIgnite&&!canApplyFrozen&&!canApplyShocked)
        {
            int random = Random.Range(0,3);
            switch(random)
            {
                case 0:
                    canApplyIgnite = true;
                    break;
                case 1:
                    canApplyFrozen = true;
                    break;
                case 2:
                    canApplyShocked = true;
                    break;
            }
        }
        _targetStats.ApplyAilments(canApplyIgnite,canApplyFrozen,canApplyShocked,this);
        if(canApplyIgnite)
        {
            _targetStats.SetupIgniteDmage(Mathf.RoundToInt(_fireDamage*0.2f));
        }
        if(canApplyShocked)
        {
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightningDamage*0.1f));
        }
    }
    /// <summary>
    /// 应用异常状态
    /// </summary>
    /// <param name="_isIgnited"></param>
    /// <param name="_isFrozend"></param>
    /// <param name="_isShocked"></param>
    public virtual void ApplyAilments(bool _isIgnited,bool _isFrozend,bool _isShocked,CharacterStats _attacker)
    {
        bool canApplyIgnite = !isIgnited&&!isFrozend&&!isShocked;
        bool canApplyFrozen = !isIgnited&&!isFrozend&&!isShocked;
        bool canApplyShocked = !isIgnited&&!isFrozend;

        if(_isIgnited&&canApplyIgnite)
        {
            isIgnited = _isIgnited;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }
        if(_isFrozend&&canApplyFrozen)
        {
            isFrozend = _isFrozend;
            chilledTimer = ailmentsDuration;
            fx.ChillFxFor(ailmentsDuration);
            GetComponent<Entity>().SlowEntityBy(20,ailmentsDuration);
        }
        if(_isShocked&&canApplyShocked)
        {
            if(!isShocked)
            {
                ApplyShocked(ailmentsDuration);
            }
            else
            {
                if(GetComponent<Player>()!=null) return;
                HitNearestTargetWithShock(_attacker);
            }
        }
    }
    private void HitNearestTargetWithShock(CharacterStats _attacker)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25f);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (Collider2D collider in colliders)
        {   
            //寻找除自身外的最近敌人
            if(Vector2.Distance(transform.position,collider.transform.position)<=0.2) continue;

            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        if(closestEnemy==null) closestEnemy = transform;
        GameObject newShockStrike = Instantiate(shockStrikePrefab,transform.position,Quaternion.identity);
        newShockStrike.GetComponent<Flash_Controller>().Setup(_attacker.shockDamage,closestEnemy.GetComponent<CharacterStats>());
    }

    #endregion

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Die()
    {
        if(isDead) return;
        currentHP = 0;
        isDead = true;
    }

    public virtual void OnEvasion()
    {

    }
    public void MakeInvincible(bool _isInvincible)=>isInvincible = _isInvincible;

    /// <summary>
    /// 闪避判定
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <returns></returns>

    private bool canEvasion(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue()+_targetStats.agility.GetValue();
        if(isShocked)
        {
            totalEvasion += 20;
        }
        
        if(Random.Range(0,100)<totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 暴击判定
    /// </summary>
    /// <returns></returns>
    private bool CanCrit()
    {
        int totalCritChance = critChance.GetValue()+agility.GetValue();
        
        if(Random.Range(0,100)<=totalCritChance)
        {
            return true;
        }
        return false;
    }
    #region 负面状态
    public virtual void ApplyShocked(float _duration)
    {
        isShocked = true;
        shockedTimer = _duration;
        fx.ShockFxFor(_duration);
    }
    public virtual void ApplyIgniteDamage()
    {
        if(igniteDamageTimer<0&&isIgnited)
        {
            DecreaseHealthBy(igniteDamage);
            if(currentHP<0) Die();
            igniteDamageTimer = igniteDamageCooldown;
        }

    }
    #endregion
    #region 获取属性
    public int GetMaxHP()=>maxHP.GetValue()+vitality.GetValue()*5;
    public int GetCurrentHP()=>currentHP;
    public int GetShieldValue()=>shieldValue;
    #endregion

    #region 设置属性
    public void SetupIgniteDmage(int _damage)=>igniteDamage = _damage;
    public void SetupShockDamage(int _damage)=>shockDamage = _damage;
    #endregion

    public virtual void IncreaseStatBy(int _modifier,float _duration, Stat _statToModify)
    {
        //Start Coroutine for stat increase
        StartCoroutine(StatModCoroutine(_modifier,_duration,_statToModify));
    }
    private IEnumerator StatModCoroutine(int _modifier,float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHP += _amount;
        if(currentHP>GetMaxHP())
        {
            currentHP = GetMaxHP();
        }
        if(OnHealthChanged!=null)
        {
            OnHealthChanged.Invoke();
        }
    }

    public void AddShield(int _shieldValue)
    {
        shieldValue += _shieldValue;
    }
    public void RemoveShield()
    {
        shieldValue =0;
    }

    

    
    public Stat GetStat(StatType _statType)
    {
        switch(_statType)
        {
            case StatType.Strength:
                return strength;
            case StatType.Agility:
                return agility;
            case StatType.Intelligence:
                return intelligence;
            case StatType.Vitality:
                return vitality;
            case StatType.Damage:
                return damage;
            case StatType.CritChance:
                return critChance;
            case StatType.CritPower:
                return critPower;
            case StatType.Health:
                return maxHP;
            case StatType.Armor:
                return armor;
            case StatType.MagicResistance:
                return magicResistance;
            case StatType.Evasion:
                return evasion;
            case StatType.FireDamage:
                return fireDamage;
            case StatType.IceDamage:
                return iceDamage;
            case StatType.LightningDamage:
                return lightningDamage;
            default:
                return null;
        }
    }

    public float GetAttackSpeed()=>attackSpeedPercentage.GetValue()/100.0f;
    public float GetMoveSpeed()=>moveSpeedPercentage.GetValue()/100.0f;
    public float GetJumpForce()=>jumpForcePercentage.GetValue()/100.0f;
    public float GetDashSpeed()=>dashSpeedPercentage.GetValue()/100.0f;
}