using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class LowerBodySM : StateMachine
{
    public bool isGrounded=false;
    public bool isRun = false;

    public Jumping jumpingState;
    public LowerIdle idleState;
    public Moving movingState;

    public LowerBodySM(PlayerInputHandler inputHandler)
    {
        idleState = new LowerIdle(this);
        jumpingState = new Jumping(this);
        movingState = new Moving(this);
        base.inputHandler = inputHandler;
    }


    protected override IBaseState GetInitialState()
    {
        return idleState;
    }
}
