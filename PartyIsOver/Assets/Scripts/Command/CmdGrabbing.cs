using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdGrabbing : PlayerCommand
{
    public CmdGrabbing(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        return actor.ActionController.InvokeEvent(data, Define.ActionEventName.Grabbing);
    }
}
