using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdDestroyJoint : PlayerCommand
{
    public CmdDestroyJoint(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeDestroyJointEvent(data);
    }
}
