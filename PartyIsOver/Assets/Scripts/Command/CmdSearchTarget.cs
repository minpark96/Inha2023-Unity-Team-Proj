using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdSearchTarget : CommandKey
{
    public CmdSearchTarget(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        return actor.ActionController.InvokeActionEvent(data, Define.ActionEventName.TargetSearch);
    }
}
