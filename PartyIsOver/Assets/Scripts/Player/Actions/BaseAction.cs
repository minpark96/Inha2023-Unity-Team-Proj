using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction
{
    public BaseAction(ActionController actions, Define.ActionEventName eventName)
    {
        actions.BindActionEvent(eventName, HandleActionEvent);
    }

    protected virtual bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext actionContext)
    {
        return true;
    }
}
