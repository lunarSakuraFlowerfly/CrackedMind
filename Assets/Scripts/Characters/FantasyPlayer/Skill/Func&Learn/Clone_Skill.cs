using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clone_Skill : BaseSkill
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashEnd;
    [SerializeField] private bool createCloneOnCounterAttack;
    [Header("Clone Duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicateClone;
    public bool crystallInsteadOfClone;

    public override void UseSkill()
    {
        base.UseSkill(); 
    }

    public void CreateClone(Transform _cloneTransform,Vector3 _offset)
    {
        if(crystallInsteadOfClone)
        {
            SkillManager.instance.crystalSkill.CreateCrystal();
            SkillManager.instance.crystalSkill.CurrentCrystalChooseRandomTarget();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_cloneTransform,cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone,chanceToDuplicateClone);
    }

    public void CreateCloneOnDashStart()
    {
        if(createCloneOnDashStart)
        {
            CreateClone(player.transform,Vector3.zero);
        }
    }
    public void CreateCloneOnDashEnd()
    {
        if(createCloneOnDashEnd)
        {
            CreateClone(player.transform,Vector3.zero);
        }
    }
    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        if(createCloneOnCounterAttack)
        {
            StartCoroutine(CloneDelayCoroutine(_enemyTransform,new Vector3(2*player.facingDirection,0)));
        }
    }

    private IEnumerator CloneDelayCoroutine(Transform _transform,Vector3 _offset)
    {
        yield return new WaitForSeconds(0.5f);
        CreateClone(_transform,_offset);
    }
}