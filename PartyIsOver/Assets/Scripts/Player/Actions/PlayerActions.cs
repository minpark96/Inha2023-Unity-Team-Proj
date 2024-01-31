using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions
{
    AnimationData _animData;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;

    public PlayerActions(AnimationData data, AnimationPlayer animPlayer, BodyHandler bodyHandler)
    {
        _animData = data;
        this._animPlayer = animPlayer;
        this._bodyHandler = bodyHandler;
        Init();
    }


    public delegate void PlayerMove(AnimationData animData, AnimationPlayer animPlayer,BodyHandler bodyHandler, in Define.PlayerDynamicData dynamicData);
    public event PlayerMove OnMove;
    public delegate void PlayerJump(AnimationData animData, AnimationPlayer animPlayer, in Define.PlayerDynamicData dynamicData);
    public event PlayerJump OnJump;

    void Init()
    {
        new JumpAction(this);
        new MoveAction(this);
    }
    
    public void InvokeJumpEvent(in Define.PlayerDynamicData data)
    {
        OnJump?.Invoke(_animData, _animPlayer, data);
    }
    public void InvokeMoveEvent(in Define.PlayerDynamicData data)
    {
        OnMove?.Invoke(_animData, _animPlayer, _bodyHandler, data);
    }
}
