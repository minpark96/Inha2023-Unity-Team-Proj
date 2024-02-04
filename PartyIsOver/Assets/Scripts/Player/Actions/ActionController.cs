using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController
{
    public ActionController(AnimationData data, AnimationPlayer animPlayer, BodyHandler bodyHandler)
    {
        _animData = data;
        this._animPlayer = animPlayer;
        this._bodyHandler = bodyHandler;
        Init();
    }

    AnimationData _animData;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;

    public delegate void PlayerJump(AnimationData animData, AnimationPlayer animPlayer, in Define.PlayerDynamicData dynamicData);
    public event PlayerJump OnJump;
    public delegate void PlayerMove(AnimationData animData, AnimationPlayer animPlayer,BodyHandler bodyHandler, in Define.PlayerDynamicData dynamicData);
    public event PlayerMove OnMove;
    public delegate void PlayerPunch(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in Define.PlayerDynamicData dynamicData);
    public event PlayerPunch OnPunch;

    void Init()
    {
        new JumpAction(this);
        new MoveAction(this);
        new PunchAction(this);
    }
    
    public void InvokeJumpEvent(in Define.PlayerDynamicData data)
    {
        OnJump?.Invoke(_animData, _animPlayer, data);
    }
    public void InvokeMoveEvent(in Define.PlayerDynamicData data)
    {
        OnMove?.Invoke(_animData, _animPlayer, _bodyHandler, data);
    }
    public void InvokePunchEvent(in Define.PlayerDynamicData data)
    {
        OnPunch?.Invoke(_animData, _animPlayer, _bodyHandler, data);
    }
}
