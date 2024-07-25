using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 동작(Action)들을 보유하고 이를 바인딩, 실행하는 클래스
 * 
 */
public class ActionController
{
    //생성시 애니메이션 데이터들을 가져와서 보유
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

    //액션 델리게이트들을 관리할 액션핸들러 선언
    public delegate bool ActionDelegate(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext dynamicData);
    private List<ActionDelegate> ActionHandlers = new List<ActionDelegate>();

    void Init()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Define.ActionEventName)).Length; i++)
        {
            ActionDelegate action = null;
            ActionHandlers.Add(action);
        }

        //액션생성과 동시에 Enum으로 접근할 수 있게 바인딩
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
    //플레이어의 현재상태와 실행하려는 Action의 이름을 받아와서 실행
    public bool InvokeActionEvent(in PlayerActionContext data,Define.ActionEventName name)
    {
        return ActionHandlers[(int)name]?.Invoke(_animData, _animPlayer, _bodyHandler, data) ?? false;
    }

    //이름에 해당하는 액션 델리게이트에 이벤트를 구독
    public void BindActionEvent(Define.ActionEventName name, ActionDelegate eventHandler)
    {
        ActionHandlers[(int)name] -= eventHandler;
        ActionHandlers[(int)name] += eventHandler;
    }
}
