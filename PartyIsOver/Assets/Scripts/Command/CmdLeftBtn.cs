using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdLeftBtn : CommandKey
{
    public CmdLeftBtn(Actor actor)
    {
        this.actor = actor;
    }
    public override bool Execute(in PlayerActionContext data)
    {
        //Left버튼이 눌렸을때 플레이어 상태에 따라 해당하는 Action을 실행
        switch (actor.GetUpperState())
        {
            case Define.PlayerState.Punch:
                return actor.ActionController.InvokeActionEvent(data,Define.ActionEventName.Punch);
            case Define.PlayerState.EquipItem:
                return actor.ActionController.InvokeActionEvent(data, Define.ActionEventName.UseItem);
            case Define.PlayerState.LiftObject:
                return actor.ActionController.InvokeActionEvent(data, Define.ActionEventName.Lift);
            default: return false;
        }
    }
}
