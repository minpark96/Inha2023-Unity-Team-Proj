using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdRightBtn : PlayerCommand
{
    public CmdRightBtn(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerContext data)
    {
        //return actor.ActionController.InvokeDropKickEvent(data);

        if(actor.GetLowerState() == Define.PlayerState.DropKick)
            return actor.ActionController.InvokeDropKickEvent(data);
        else
            return actor.ActionController.InvokeThrowEvent(data);
    }
}
