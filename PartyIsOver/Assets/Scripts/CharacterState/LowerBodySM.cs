using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static Define;

public class LowerBodySM : StateMachine
{
    public bool isGrounded=false;
    public bool isRun = false;

    public Jumping jumpingState;
    public LowerIdle idleState;
    public Moving movingState;

    public BodyPose leftArmPose;
    public BodyPose rightArmPose;
    public BodyPose leftLegPose;
    public BodyPose rightLegPose;

    int[] _aryBodyPose = new int[4];

    public LowerBodySM(PlayerInputHandler inputHandler)
    {
        idleState = new LowerIdle(this);
        jumpingState = new Jumping(this);
        movingState = new Moving(this);
        base.InputHandler = inputHandler;
        base.Init();
    }

    public int[] GetBodyPose()
    {
        _aryBodyPose[0] = (int)leftArmPose;
        _aryBodyPose[1] = (int)rightArmPose;
        _aryBodyPose[2] = (int)leftLegPose;
        _aryBodyPose[3] = (int)rightLegPose;
        return _aryBodyPose;
    }

    protected override IBaseState GetInitialState()
    {
        return idleState;
    }
}
