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


    public delegate void PlayerMove(AnimationData animData, AnimationPlayer animPlayer,BodyHandler bodyHandler, in Define.PlayerDynamicData dynamicData);
    public event PlayerMove OnMove;
    public delegate void PlayerJump(AnimationData animData, AnimationPlayer animPlayer, in Define.PlayerDynamicData dynamicData);
    public event PlayerJump OnJump;

    
    public void InvokeJumpEvent(in Define.PlayerDynamicData data)
    {
        OnJump?.Invoke(animData, animPlayer, data);
    }
    public void InvokeMoveEvent(in Define.PlayerDynamicData data)
    {
        OnMove?.Invoke(animData, animPlayer, bodyHandler, data);
    }
}
