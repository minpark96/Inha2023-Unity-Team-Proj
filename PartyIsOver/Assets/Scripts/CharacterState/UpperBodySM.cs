using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class UpperBodySM : StateMachine
{
    public Define.Side ReadySide = Define.Side.Left;
    public bool IsAttacking = false;


    public UpperIdle IdleState;
    public PunchReady PunchReadyState;
    public Punch PunchState;
    public Grabbing GrabbingState;

    public UpperBodySM(PlayerInputHandler inputHandler)
    {
        IdleState = new UpperIdle(this);
        PunchReadyState = new PunchReady(this);
        PunchState = new Punch(this);
        GrabbingState = new Grabbing(this);

        base.InputHandler = inputHandler;
        base.Init();
    }



    protected override IBaseState GetInitialState()
    {
        return IdleState;
    }
}
