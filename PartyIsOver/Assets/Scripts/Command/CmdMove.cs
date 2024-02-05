using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CmdMove : PlayerCommand
{
    public CmdMove(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in Define.PlayerDynamicData data)
    {
        return actor.ActionController.InvokeMoveEvent(data);
    }

}
