using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdSkill : PlayerCommand
{
    public CmdSkill(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in Define.PlayerDynamicData data)
    {
        return actor.ActionController.InvokeSkillEvent(data);
    }
}
