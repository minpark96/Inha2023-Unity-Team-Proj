using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCharge:PlayerCommand
{
    public CmdCharge(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        return actor.ActionController.InvokeEvent(data, Define.ActionEventName.ChargeReady);
    }
}
