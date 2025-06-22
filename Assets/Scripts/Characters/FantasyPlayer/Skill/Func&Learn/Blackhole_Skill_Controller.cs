using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    private bool canGrow = true;
    private bool canShrink = false;
    private bool playerCanDisapear = true;

    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private int attackAmounts = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;
    private float blackholeTimer;
    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKeys = new List<GameObject>();
    public bool playerCanExitState{get;private set;}

    public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeed,int _attackAmounts,float _cloneAttackCooldown,float _BlackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        attackAmounts = _attackAmounts;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _BlackholeDuration;

        if(SkillManager.instance.cloneSkill.crystallInsteadOfClone)
        {
            playerCanDisapear = false;
        }
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;
        if(blackholeTimer<0)
        {
            blackholeTimer = Mathf.Infinity;
            if(targets.Count>0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackholeAbility();
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();
        if(canGrow&&!canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale,new Vector2(maxSize,maxSize),growSpeed*Time.deltaTime);
        }
        if(canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale,new Vector2(-1,-1),shrinkSpeed*Time.deltaTime);
            if(transform.localScale.x<0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void ReleaseCloneAttack()
    {
        if(targets.Count<=0)
            return;
        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;
        if(playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true);
        }
    }
    private void CloneAttackLogic()
    {
        if(cloneAttackTimer<0 && cloneAttackReleased && attackAmounts>0)
        {
            cloneAttackTimer = cloneAttackCooldown;
            int randomIndex = Random.Range(0,targets.Count);

            float xOffset;
            if(Random.Range(0,100)>50)
            {
                xOffset = 1;
            }
            else
            {
                xOffset = -1;
            }
            if(SkillManager.instance.cloneSkill.crystallInsteadOfClone)
            {
                SkillManager.instance.crystalSkill.CreateCrystal();
                SkillManager.instance.crystalSkill.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.cloneSkill.CreateClone(targets[randomIndex],new Vector3(xOffset,0,0));
            }
            attackAmounts--;
            if(attackAmounts<=0)
            {
                Invoke("FinishBlackholeAbility",1);
            }
        }
    }

    private void FinishBlackholeAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        //PlayerManager.instance.player.ExitBlackholeAbility();
        canShrink = true;
        cloneAttackReleased = false;
    }
    private void DestroyHotKeys()
    {
        if(createdHotKeys.Count<=0)
            return;
        for(int i=0;i<createdHotKeys.Count;i++)
        {
            Destroy(createdHotKeys[i]);
        }
        createdHotKeys.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!=null)
        {
            collision.GetComponent<Enemy>().FreezeTimer(true);
            createHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)=>collision.GetComponent<Enemy>()?.FreezeTimer(false);

    private void createHotKey(Collider2D collision)
    {
        if(keyCodeList.Count<=0)
            return;
        if(!canCreateHotKeys)
            return;    
        GameObject newHotKey = Instantiate(hotKeyPrefab,collision.transform.position+new Vector3(0,2,0),Quaternion.identity);
        KeyCode choosenKey = keyCodeList[Random.Range(0,keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);
        createdHotKeys.Add(newHotKey);
        Blackhole_Hotkey_Controller newHotKeyController = newHotKey.GetComponent<Blackhole_Hotkey_Controller>();
        newHotKeyController.SetupHotKey(choosenKey,collision.transform,this);
    }

    public void AddEnemyToList(Transform _enemyTransform)=>targets.Add(_enemyTransform);
}
