using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdRightBtn : PlayerCommand
{
    public CmdRightBtn(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        if(actor.GetLowerState() == Define.PlayerState.DropKick)
            return actor.ActionController.InvokeEvent(data, Define.ActionEventName.DropKick);
        else
            return actor.ActionController.InvokeEvent(data, Define.ActionEventName.Throw);
    }
}
