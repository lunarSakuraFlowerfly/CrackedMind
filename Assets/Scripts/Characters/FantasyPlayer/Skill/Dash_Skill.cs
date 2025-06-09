using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_Skill : BaseSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("在周围创造一个克隆体");
    }
}