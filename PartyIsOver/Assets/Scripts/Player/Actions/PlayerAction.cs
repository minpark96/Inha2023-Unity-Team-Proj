using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{
    public PlayerAction(ActionController actions, Define.ActionEventName name)
    {
        actions.BindEvent(name, HandleActionEvent);
    }

    protected virtual bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        return true;
    }
}
