using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCharge:PlayerCommand
{
    public CmdCharge(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeChargeEvent(data);
    }
}
