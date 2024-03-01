using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Moving : BodyState
{
    private LowerBodySM _sm;


    private float _cycleTimer = 0;
    private float _cycleSpeed;

    public Moving(StateMachine stateMachine) : base(PlayerState.Moving, stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            _sm.LeftLegPose = BodyPose.Bent;
            _sm.RightLegPose = BodyPose.Straight;
            _sm.LeftArmPose = BodyPose.Straight;
            _sm.RightArmPose = BodyPose.Bent;
        }
        else
        {
            _sm.LeftLegPose = BodyPose.Straight;
            _sm.RightLegPose = BodyPose.Bent;
            _sm.LeftArmPose = BodyPose.Bent;
            _sm.RightArmPose = BodyPose.Straight;
        }
    }

    public override void Exit()
    {
        _sm.IsRun = false;
    }

    public override void UpdateLogic()
    {
        if (!_sm.InputHandler.IsMoveInput())
            _sm.ChangeState(_sm.IdleState);
    }

    public override void UpdatePhysics()
    {
        if (_sm.IsRun)
            _cycleSpeed = 0.1f;
        else
            _cycleSpeed = 0.15f;

        RunCycleUpdate();
    }

    public override void GetInput()
    {
        IsMoveKeyInput();
  
        if (_sm.InputHandler.InputCommnadKey(Define.COMMAND_KEY.Jump, Define.GetKeyType.Down))
        {
            _sm.ChangeState(_sm.JumpingState);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _sm.IsRun = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _sm.IsRun = false;
        }
    }

    private void RunCycleUpdate()
    {
        if (_cycleTimer < _cycleSpeed)
        {
            _cycleTimer += Time.deltaTime;
            return;
        }
        _cycleTimer = 0f;
        int num = (int)_sm.LeftArmPose;
        num++;
        _sm.LeftArmPose = ((num <= 3) ? ((BodyPose)num) : BodyPose.Bent);
        int num2 = (int)_sm.RightArmPose;
        num2++;
        _sm.RightArmPose = ((num2 <= 3) ? ((BodyPose)num2) : BodyPose.Bent);
        int num3 = (int)_sm.LeftLegPose;
        num3++;
        _sm.LeftLegPose = ((num3 <= 3) ? ((BodyPose)num3) : BodyPose.Bent);
        int num4 = (int)_sm.RightLegPose;
        num4++;
        _sm.RightLegPose = ((num4 <= 3) ? ((BodyPose)num4) : BodyPose.Bent);
    }
}
