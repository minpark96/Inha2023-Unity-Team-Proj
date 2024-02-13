using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdHeadButt : PlayerCommand
{
    public CmdHeadButt(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in Define.PlayerDynamicData data)
    {
        return actor.ActionController.InvokeHeadButtEvent(data);
    }
}
