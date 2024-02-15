using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdSearchTarget : PlayerCommand
{
    public CmdSearchTarget(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeTargetSearchEvent(data);
    }
}
