using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdFixJoint : CommandKey
{
    public CmdFixJoint(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        return actor.ActionController.InvokeActionEvent(data, Define.ActionEventName.JointFix);
    }
}
