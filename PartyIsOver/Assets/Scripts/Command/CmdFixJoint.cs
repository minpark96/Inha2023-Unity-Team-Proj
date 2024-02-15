using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdFixJoint : PlayerCommand
{
    public CmdFixJoint(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        return actor.ActionController.InvokeFixJointEvent(data);
    }
}
