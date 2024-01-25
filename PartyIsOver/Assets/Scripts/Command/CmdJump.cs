using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdJump : PlayerCommand
{
    public CmdJump(Actor actor)
    {
        this.actor = actor;
    }

    public override void Execute(in Define.PlayerDynamicData data)
    {
        actor.PlayerActions.InvokeJumpEvent(data);
    }
}
