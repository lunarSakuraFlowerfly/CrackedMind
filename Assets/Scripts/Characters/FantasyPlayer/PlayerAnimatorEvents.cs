using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAnimatorEvents : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }
    private void AnimationTrigger()=>player.AnimationTrigger();
}
