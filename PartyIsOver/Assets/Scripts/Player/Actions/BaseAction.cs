using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//액션의 베이스가 되는 추상클래스

public abstract class BaseAction
{
    //생성시 해당하는 이름의 Action에 이벤트를 바인딩
    public BaseAction(ActionController actions, Define.ActionEventName eventName)
    {
        actions.BindActionEvent(eventName, HandleActionEvent);
    }

    //액션이 Invoke 됐을때 실제 실행되는 함수
    protected virtual bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext actionContext)
    {
        return true;
    }
}
