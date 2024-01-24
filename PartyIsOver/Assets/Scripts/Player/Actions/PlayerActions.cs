using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions
{
    AnimationData animData;
    AnimationPlayer animPlayer;
    BodyHandler bodyHandler;

    public PlayerActions(AnimationData data, AnimationPlayer animPlayer, BodyHandler bodyHandler)
    {
        animData = data;
        this.animPlayer = animPlayer;
        this.bodyHandler = bodyHandler;
    }


    public delegate void PlayerMove(AnimationData data, AnimationPlayer animPlayer,BodyHandler bodyHandler, Vector3 dir);
    public event PlayerMove OnMove;
    public delegate void PlayerJump(AnimationData data, AnimationPlayer animPlayer, Vector3 dir);
    public event PlayerJump OnJump;

    
    public void InvokeJumpEvent(Vector3 dir)
    {
        OnJump?.Invoke(animData, animPlayer, dir);
    }
    public void InvokeMoveEvent(Vector3 dir)
    {
        OnMove?.Invoke(animData, animPlayer, bodyHandler, dir);
    }
}
