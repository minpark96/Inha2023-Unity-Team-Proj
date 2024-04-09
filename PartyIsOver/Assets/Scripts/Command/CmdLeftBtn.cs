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
        //Left��ư�� �������� �÷��̾� ���¿� ���� �ش��ϴ� Action�� ����
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
