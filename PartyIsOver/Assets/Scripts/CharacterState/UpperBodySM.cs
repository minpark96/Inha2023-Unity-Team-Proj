using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class UpperBodySM : StateMachine
{
    public PlayerContext Context;

    public Define.Side ReadySide = Define.Side.Left;
    public bool IsUpperActionProgress = false;
    public bool IsMeowPunch = false;
    public bool IsEquipItem = false;
    public bool IsGrabbingInProgress=false;
    public Vector3 RightTargetDir = Vector3.zero;
    public Vector3 LeftTargetDir = Vector3.zero;
    public float ItemCoolTime;

    public HandChecker LeftHandCheckter;
    public HandChecker RightHandCheckter;

    public IBaseState IdleState;
    public IBaseState PunchReadyState;
    public IBaseState PunchState;
    public IBaseState GrabbingState;
    public IBaseState SkillReadyState;
    public IBaseState SkillState;
    public IBaseState HeadButtState;


    public UpperBodySM(PlayerInputHandler inputHandler, PlayerContext playerContext, HandChecker left, HandChecker right)
    {
        Context = playerContext;
        InputHandler = inputHandler;

        IdleState = new UpperIdle(this);
        PunchReadyState = new PunchReady(this);
        PunchState = new Punch(this);
        GrabbingState = new Grabbing(this);
        SkillReadyState = new SkillReady(this);
        SkillState = new NuclearPunch(this);
        HeadButtState = new HeadButt(this);
        LeftHandCheckter = left;
        RightHandCheckter = right;
        Init();
    }

    protected override IBaseState GetInitialState()
    {
        return IdleState;
    }
}
