using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static Define;

public class LowerBodySM : StateMachine
{
    public PlayerActionContext PlayerContext;

    public bool IsGrounded=false;
    public bool IsRun = false;

    public BaseState JumpingState;
    public BaseState IdleState;
    public BaseState MovingState;
    public BaseState DropKickState;


    public BodyPose LeftArmPose;
    public BodyPose RightArmPose;
    public BodyPose LeftLegPose;
    public BodyPose RightLegPose;

    int[] _aryBodyPose = new int[4];


    public LowerBodySM(PlayerInputHandler inputHandler, PlayerActionContext playerContext, CommandDelegate cmdReserveHandler)
    {
        IdleState = new LowerIdle(this);
        JumpingState = new Jumping(this);
        MovingState = new Moving(this);
        DropKickState = new DropKick(this);

        CommandReserveHandler -= cmdReserveHandler;
        CommandReserveHandler += cmdReserveHandler;
        PlayerContext = playerContext;
        base.InputHandler = inputHandler;
        base.Init();
    }

    public int[] GetBodyPose()
    {
        _aryBodyPose[0] = (int)LeftArmPose;
        _aryBodyPose[1] = (int)RightArmPose;
        _aryBodyPose[2] = (int)LeftLegPose;
        _aryBodyPose[3] = (int)RightLegPose;
        return _aryBodyPose;
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }
}
