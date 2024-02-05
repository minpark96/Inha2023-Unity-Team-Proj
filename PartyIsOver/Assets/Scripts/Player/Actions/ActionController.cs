using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController
{
    public ActionController(AnimationData data, AnimationPlayer animPlayer, BodyHandler bodyHandler)
    {
        _animData = data;
        _animPlayer = animPlayer;
        _bodyHandler = bodyHandler;
        Init();
    }

    AnimationData _animData;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;

    public delegate bool ActionDelegate(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in Define.PlayerDynamicData dynamicData);
    public event ActionDelegate OnJump;
    public event ActionDelegate OnMove;
    public event ActionDelegate OnPunch;
    public event ActionDelegate OnSkill;

    public delegate void ActionEndNotify();
    public event ActionEndNotify OnActionEnd;

    void Init()
    {
        new JumpAction(this);
        new MoveAction(this);
        new PunchAction(this);
        new SkillAction(this);
    }
    
    public bool InvokeJumpEvent(in Define.PlayerDynamicData data)
    {
        return OnJump?.Invoke(_animData, _animPlayer,_bodyHandler, data) ?? false;
    }
    public bool InvokeMoveEvent(in Define.PlayerDynamicData data)
    {
        return OnMove?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokePunchEvent(in Define.PlayerDynamicData data)
    {
        return OnPunch?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeSkillEvent(in Define.PlayerDynamicData data)
    {
        return OnSkill?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }

    public void UpperActionEnd()
    {
        OnActionEnd?.Invoke();
    }
}
