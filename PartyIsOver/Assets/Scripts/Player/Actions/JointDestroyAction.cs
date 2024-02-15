using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JointDestroyAction
{
    public JointDestroyAction(ActionController actions)
    {
        actions.OnJointDestroy -= HandleJointDestroyEvent;
        actions.OnJointDestroy += HandleJointDestroyEvent;
    }

    bool HandleJointDestroyEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {


        return true;
    }
}
