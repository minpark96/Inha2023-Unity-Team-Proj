using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{


    bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {

        return true;
    }
}
