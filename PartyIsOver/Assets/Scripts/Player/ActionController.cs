using System;
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
    private List<ActionDelegate> ActionHandlers = new List<ActionDelegate>();

    void Init()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Define.ActionEventName)).Length; i++)
        {
            ActionDelegate action = null;
            ActionHandlers.Add(action);
        }

        new JumpAction          (this, Define.ActionEventName.Jump);
        new MoveAction          (this, Define.ActionEventName.Move);
        new PunchAction         (this, Define.ActionEventName.Punch);
        new SkillAction         (this, Define.ActionEventName.Skill);
        new ChargeReadyAction   (this, Define.ActionEventName.ChargeReady);
        new ResetChargeAction   (this, Define.ActionEventName.ResetCharge);
        new HeadButtAction      (this, Define.ActionEventName.HeadButt);
        new DropKickAction      (this, Define.ActionEventName.DropKick);
        new GrabbingAction      (this, Define.ActionEventName.Grabbing);
        new JointFixAction      (this, Define.ActionEventName.JointFix);
        new JointDestroyAction  (this, Define.ActionEventName.JointDestroy);
        new SearchAction        (this, Define.ActionEventName.TargetSearch);
        new ItemUseAction       (this, Define.ActionEventName.UseItem);
        new ThrowAction         (this, Define.ActionEventName.Throw);
        new LiftAction          (this, Define.ActionEventName.Lift);
    }
    public bool InvokeActionEvent(in PlayerActionContext data,Define.ActionEventName name)
    {
        return ActionHandlers[(int)name]?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }

    public void BindActionEvent(Define.ActionEventName name, ActionDelegate eventHandler)
    {
        ActionHandlers[(int)name] -= eventHandler;
        ActionHandlers[(int)name] += eventHandler;
    }
}
