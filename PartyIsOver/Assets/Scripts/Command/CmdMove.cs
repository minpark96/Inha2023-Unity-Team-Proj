using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Actor;

public class CmdMove : PlayerCommand
{
    public CmdMove(Actor actor)
    {
        this.actor = actor;
    }
    public override void Execute(Vector3 moveDir = default)
    {
        actor.PlayerActions.InvokeMoveEvent(moveDir);
    }

}
