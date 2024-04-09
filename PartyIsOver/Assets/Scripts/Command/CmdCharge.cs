using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCharge:CommandKey
{
    public CmdCharge(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        return actor.ActionController.InvokeActionEvent(data, Define.ActionEventName.ChargeReady);
    }
}
