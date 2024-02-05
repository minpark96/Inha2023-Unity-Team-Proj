using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class UpperBodySM : StateMachine
{
    public Define.Side ReadySide = Define.Side.Left;
    public bool IsActionProgress = false;

    //public IBaseState IdleState;
    //public IBaseState PunchReadyState;
    //public IBaseState PunchState;
    //public IBaseState GrabbingState;
    //public IBaseState SkillState;

    public UpperIdle IdleState;
    public PunchReady PunchReadyState;
    public Punch PunchState;
    public Grabbing GrabbingState;
    public SkillReady SkillReadyState;
    public IBaseState SkillState;

    public UpperBodySM(PlayerInputHandler inputHandler)
    {
        IdleState = new UpperIdle(this);
        PunchReadyState = new PunchReady(this);
        PunchState = new Punch(this);
        GrabbingState = new Grabbing(this);
        SkillReadyState = new SkillReady(this);
        SkillState = new NuclearPunch(this);
        base.InputHandler = inputHandler;
        base.Init();
    }



    protected override IBaseState GetInitialState()
    {
        return IdleState;
    }
}
