using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdResetCharge : PlayerCommand
{
    public CmdResetCharge(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeResetChargeEvent(data);
    }
}