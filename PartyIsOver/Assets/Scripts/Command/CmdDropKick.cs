using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdDropKick : PlayerCommand
{
    public CmdDropKick(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeDropKickEvent(data);
    }
}
