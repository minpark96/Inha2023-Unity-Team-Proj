using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{


    bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {

        return true;
    }
}
