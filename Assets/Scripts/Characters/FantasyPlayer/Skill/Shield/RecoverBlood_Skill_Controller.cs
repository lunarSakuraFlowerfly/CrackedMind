using UnityEngine;
public class RecoverBlood_Skill_Controller : MonoBehaviour
{
    private float recoverBloodDuration;//恢复血量持续时间
    private float recoverBloodInterval;//每隔多少秒恢复一次血量
    private int recoverBloodMaxPercent;
    private int recoverBloodMinPercent;

    private Animator animator;

    private int recoverBloodAmount;
    private float recoverBloodTimer;
    private int intervalCount;
    private int recoverBloodAmountPerInterval;


    private bool isReadyToHeal;
    public void Setup(float _recoverBloodDuration,float _recoverBloodInterval,int _recoverBloodMaxPercent,int _recoverBloodMinPercent)
    {
        recoverBloodDuration = _recoverBloodDuration;
        recoverBloodInterval = _recoverBloodInterval;
        recoverBloodMaxPercent = _recoverBloodMaxPercent;
        recoverBloodMinPercent = _recoverBloodMinPercent;
        int recoverBloodPercent = Random.Range(recoverBloodMinPercent,recoverBloodMaxPercent);
        recoverBloodAmount = Mathf.RoundToInt(PlayerManager.instance.player.Stats.maxHP.GetValue() * recoverBloodPercent / 100.0f);
        intervalCount = Mathf.RoundToInt(recoverBloodDuration / recoverBloodInterval);
        recoverBloodAmountPerInterval = Mathf.RoundToInt(recoverBloodAmount / intervalCount);
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(isReadyToHeal)
        {
            recoverBloodDuration -= Time.deltaTime;
            if(recoverBloodDuration <= 0)
            {
                isReadyToHeal = false;
                animator.SetTrigger("EndTrigger");
            }
        }
        
        recoverBloodTimer += Time.deltaTime;
        if(recoverBloodTimer >= recoverBloodInterval)
        {
            RecoverBlood();
            recoverBloodTimer = 0;
        }
    }
    private void RecoverBlood()
    {
        PlayerManager.instance.player.Stats.IncreaseHealthBy(recoverBloodAmountPerInterval);
    }


    private void DestorySelfTirgger()
    {
        Destroy(gameObject);
    }

    private void ReadyToHealTrigger()
    {
        isReadyToHeal = true;
    }

    
}