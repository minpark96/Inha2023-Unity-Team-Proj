using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdLeftBtn : PlayerCommand
{
    public CmdLeftBtn(Actor actor)
    {
        this.actor = actor;
    }
    public override void Execute(in Define.PlayerDynamicData data)
    {
        actor.ActionController.InvokeMoveEvent(data);
    }
}
