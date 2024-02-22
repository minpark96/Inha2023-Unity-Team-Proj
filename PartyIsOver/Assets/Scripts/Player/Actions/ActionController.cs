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

    public delegate bool ActionDelegate(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext dynamicData);
    public event ActionDelegate OnJump;
    public event ActionDelegate OnMove;
    public event ActionDelegate OnPunch;
    public event ActionDelegate OnSkill;
    public event ActionDelegate OnChargeReady;
    public event ActionDelegate OnResetCharge;
    public event ActionDelegate OnHeadButt;
    public event ActionDelegate OnDropKick;
    public event ActionDelegate OnGrabbing;
    public event ActionDelegate OnTargetSearch;
    public event ActionDelegate OnJointFix;
    public event ActionDelegate OnJointDestroy;
    public event ActionDelegate OnUseItem;



    public delegate void ActionEndNotify();
    public event ActionEndNotify OnUpperActionEnd;
    public event ActionEndNotify OnUpperActionStart;


    void Init()
    {
        new JumpAction(this);
        new MoveAction(this);
        new PunchAction(this);
        new SkillAction(this);
        new ChargeReadyAction(this);
        new ResetChargeAction(this);
        new HeadButtAction(this);
        new DropKickAction(this);
        new GrabbingAction(this);
        new JointFixAction(this);
        new JointDestroyAction(this);
        new SearchAction(this);
        new ItemUseAction(this);
    }
    
    public bool InvokeJumpEvent(in PlayerContext data)
    {
        return OnJump?.Invoke(_animData, _animPlayer,_bodyHandler, data) ?? false;
    }
    public bool InvokeMoveEvent(in PlayerContext data)
    {
        return OnMove?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokePunchEvent(in PlayerContext data)
    {
        return OnPunch?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeSkillEvent(in PlayerContext data)
    {
        return OnSkill?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeChargeEvent(in PlayerContext data)
    {
        return OnChargeReady?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeResetChargeEvent(in PlayerContext data)
    {
        return OnResetCharge?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeHeadButtEvent(in PlayerContext data)
    {
        return OnHeadButt?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeDropKickEvent(in PlayerContext data)
    {
        return OnDropKick?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeGrabbingEvent(in PlayerContext data)
    {
        return OnGrabbing?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeTargetSearchEvent(in PlayerContext data)
    {
        return OnTargetSearch?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeFixJointEvent(in PlayerContext data)
    {
        return OnJointFix?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeDestroyJointEvent(in PlayerContext data)
    {
        return OnJointDestroy?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeUseItemEvent(in PlayerContext data)
    {
        return OnUseItem?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }

    public void UpperActionEnd()
    {
        OnUpperActionEnd?.Invoke();
    }
    public void UpperActionStart()
    {
        OnUpperActionStart?.Invoke();
    }

}
