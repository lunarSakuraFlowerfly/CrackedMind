using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimatorEvents : MonoBehaviour
{
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    public void AnimationTrigger()
    {
        enemy.stateMachine.currentState.AnimationFinishTrigger();
    }
}
