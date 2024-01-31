using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Moving : BodyState
{
    private LowerBodySM _sm;


    private float _cycleTimer = 0;
    private float _cycleSpeed;

    public Moving(StateMachine stateMachine) : base("MovingState", stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            _sm.leftLegPose = BodyPose.Bent;
            _sm.rightLegPose = BodyPose.Straight;
            _sm.leftArmPose = BodyPose.Straight;
            _sm.rightArmPose = BodyPose.Bent;
        }
        else
        {
            _sm.leftLegPose = BodyPose.Straight;
            _sm.rightLegPose = BodyPose.Bent;
            _sm.leftArmPose = BodyPose.Bent;
            _sm.rightArmPose = BodyPose.Straight;
        }
    }

    public override void Exit()
    {
        _sm.isRun = false;
    }

    public override void UpdateLogic()
    {
        if (!_sm.InputHandler.IsMoveInput())
            _sm.ChangeState(_sm.idleState);
    }

    public override void UpdatePhysics()
    {
        if (_sm.isRun)
            _cycleSpeed = 0.1f;
        else
            _cycleSpeed = 0.15f;

        RunCycleUpdate();
    }

    public override void GetInput()
    {
        IsMoveKeyInput();
  
        if (_sm.InputHandler.InputGetDownKey(KeyCode.Space, Define.GetKeyType.Down))
        {
            _sm.ChangeState(_sm.jumpingState);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _sm.isRun = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _sm.isRun = false;
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
        int num = (int)_sm.leftArmPose;
        num++;
        _sm.leftArmPose = ((num <= 3) ? ((BodyPose)num) : BodyPose.Bent);
        int num2 = (int)_sm.rightArmPose;
        num2++;
        _sm.rightArmPose = ((num2 <= 3) ? ((BodyPose)num2) : BodyPose.Bent);
        int num3 = (int)_sm.leftLegPose;
        num3++;
        _sm.leftLegPose = ((num3 <= 3) ? ((BodyPose)num3) : BodyPose.Bent);
        int num4 = (int)_sm.rightLegPose;
        num4++;
        _sm.rightLegPose = ((num4 <= 3) ? ((BodyPose)num4) : BodyPose.Bent);
    }



}
