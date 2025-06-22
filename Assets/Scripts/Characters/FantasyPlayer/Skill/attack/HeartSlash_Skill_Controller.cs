using UnityEngine;

public class HeartSlash_Skill_Controller : MonoBehaviour
{
    private float effectDuration;
    private int attackSpeedBuff;//攻击速度百分比
    private float fadeInDuration;
    private float fadeOutDuration;
    private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = new Color(1,1,1,0);
    }
    public void SetHeartSlash(float _effectDuration,int _attackSpeedBuff,float _fadeInDuration,float _fadeOutDuration)
    {
        effectDuration = _effectDuration;
        attackSpeedBuff = _attackSpeedBuff;
        fadeInDuration = _fadeInDuration;
        fadeOutDuration = _fadeOutDuration;
        SetAttackSpeedBuff();
    }
    private void SetAttackSpeedBuff()
    {
        PlayerManager.instance.player.GetComponent<PlayerStats>().attackSpeedPercentage.AddModifier(attackSpeedBuff);
    }
    private void RemoveAttackSpeedBuff()
    {
        PlayerManager.instance.player.GetComponent<PlayerStats>().attackSpeedPercentage.RemoveModifier(attackSpeedBuff);
    }
    private void Update()
    {
        effectDuration -= Time.deltaTime;
        //fade in
        if(sr.color.a<1&&effectDuration>0)
        {
            sr.color = new Color(1,1,1,sr.color.a+Time.deltaTime/fadeInDuration);
        }
        if(effectDuration<0)
        {
            sr.color = new Color(1,1,1,sr.color.a-Time.deltaTime/fadeOutDuration);
            if(sr.color.a<=0)
            {
                RemoveAttackSpeedBuff();
                Destroy(gameObject);
            }
        }

    }
}