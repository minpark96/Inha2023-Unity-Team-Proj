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
    public override void Execute(in Define.PlayerDynamicData data)
    {
        actor.PlayerActions.InvokeMoveEvent(data);
    }

}
