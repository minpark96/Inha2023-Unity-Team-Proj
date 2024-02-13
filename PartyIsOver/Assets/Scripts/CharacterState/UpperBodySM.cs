using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class UpperBodySM : StateMachine
{
    public Define.Side ReadySide = Define.Side.Left;
    public bool IsUpperActionProgress = false;
    public bool IsMeowPunch = false;
    public bool IsEquipItem = false;

    public IBaseState IdleState;
    public IBaseState PunchReadyState;
    public IBaseState PunchState;
    public IBaseState GrabbingState;
    public IBaseState SkillReadyState;
    public IBaseState SkillState;
    public IBaseState HeadButtState;


    public UpperBodySM(PlayerInputHandler inputHandler)
    {
        IdleState = new UpperIdle(this);
        PunchReadyState = new PunchReady(this);
        PunchState = new Punch(this);
        GrabbingState = new Grabbing(this);
        SkillReadyState = new SkillReady(this);
        SkillState = new NuclearPunch(this);
        HeadButtState = new HeadButt(this);
        base.InputHandler = inputHandler;
        base.Init();
    }



    protected override IBaseState GetInitialState()
    {
        return IdleState;
    }
}
