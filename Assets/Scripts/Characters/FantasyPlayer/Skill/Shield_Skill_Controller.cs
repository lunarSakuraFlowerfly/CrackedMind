using UnityEngine;
public class Shield_Skill_Controller : MonoBehaviour
{
    private int shieldValue;
    private float shieldDuration;
    private float growSpeed;
    private float shrinkSpeed;
    private bool canGrow = true;
    private bool canShrink = false;
    [SerializeField] private Vector2 startScale = new Vector2(0,0);
    [SerializeField] private Vector2 endScale = new Vector2(1,1.5f);
    private Vector2 currentScale=new Vector2(0,0);

    public void SetupShield(int _shieldValue,float _shieldDuration,float _growSpeed,float _shrinkSpeed)
    {
        shieldValue = _shieldValue;
        shieldDuration = _shieldDuration;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        AddShieldToPlayer();
    }

    private void Update()
    {
        shieldDuration -= Time.deltaTime;
        if(canGrow)
        {
            //从（0，0）->（1，1.5）
            Debug.Log("growSpeed: " + growSpeed);
            currentScale = Vector3.Lerp(currentScale,endScale,Time.deltaTime*growSpeed);
            Debug.Log("currentScale: " + currentScale);
      
            if(Vector3.Distance(currentScale,endScale)<0.01f)
            {
                canGrow = false;
            }
        }
        if(shieldDuration<=0||PlayerManager.instance.player.Stats.GetShieldValue()<=0)
        {
            canShrink = true;
        }
        if(canShrink)
        {
            currentScale = Vector2.Lerp(currentScale,startScale,Time.deltaTime*shrinkSpeed);
            if(Vector3.Distance(currentScale,startScale)<0.01f)
            {
                PlayerManager.instance.player.Stats.RemoveShield();
                Destroy(gameObject);
            }
        }
        transform.localScale = new Vector3(currentScale.x,currentScale.y,1);
    }

    //将护盾添加给角色
    private void AddShieldToPlayer()
    {
        PlayerManager.instance.player.Stats.AddShield(shieldValue);
    }
    private void OnDestroy()
    {

    }
}