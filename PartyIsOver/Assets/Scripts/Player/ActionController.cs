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

    public delegate bool ActionDelegate(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext dynamicData);
    //public event ActionDelegate[] ActionEvents;
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
    public event ActionDelegate OnThrow;
    public event ActionDelegate OnLift;


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
        new ThrowAction(this);
        new LiftAction(this);
    }
    
    public bool InvokeJumpEvent(in PlayerActionContext data)
    {
        return OnJump?.Invoke(_animData, _animPlayer,_bodyHandler, data) ?? false;
    }
    public bool InvokeMoveEvent(in PlayerActionContext data)
    {
        return OnMove?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokePunchEvent(in PlayerActionContext data)
    {
        return OnPunch?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeSkillEvent(in PlayerActionContext data)
    {
        return OnSkill?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeChargeEvent(in PlayerActionContext data)
    {
        return OnChargeReady?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeResetChargeEvent(in PlayerActionContext data)
    {
        return OnResetCharge?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeHeadButtEvent(in PlayerActionContext data)
    {
        return OnHeadButt?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeDropKickEvent(in PlayerActionContext data)
    {
        return OnDropKick?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeGrabbingEvent(in PlayerActionContext data)
    {
        return OnGrabbing?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeTargetSearchEvent(in PlayerActionContext data)
    {
        return OnTargetSearch?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeFixJointEvent(in PlayerActionContext data)
    {
        return OnJointFix?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeDestroyJointEvent(in PlayerActionContext data)
    {
        return OnJointDestroy?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeUseItemEvent(in PlayerActionContext data)
    {
        return OnUseItem?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeThrowEvent(in PlayerActionContext data)
    {
        return OnThrow?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
    public bool InvokeLiftEvent(in PlayerActionContext data)
    {
        return OnLift?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }
}
