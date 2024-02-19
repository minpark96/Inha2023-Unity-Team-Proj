using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdJump : PlayerCommand
{
    public CmdJump(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeJumpEvent(data);
    }
}