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
            _sm.inputHandler.leftLegPose = BodyPose.Bent;
            _sm.inputHandler.rightLegPose = BodyPose.Straight;
            _sm.inputHandler.leftArmPose = BodyPose.Straight;
            _sm.inputHandler.rightArmPose = BodyPose.Bent;
        }
        else
        {
            _sm.inputHandler.leftLegPose = BodyPose.Straight;
            _sm.inputHandler.rightLegPose = BodyPose.Bent;
            _sm.inputHandler.leftArmPose = BodyPose.Bent;
            _sm.inputHandler.rightArmPose = BodyPose.Straight;
        }
    }

    public override void UpdateLogic()
    {
        if (!_sm.inputHandler.IsMoveInput())
            _sm.ChangeState(_sm.idleState);

        if (_sm.isRun)
            _cycleSpeed = 0.1f;
        else
            _cycleSpeed = 0.15f;

        RunCycleUpdate();
    }

    public override void GetInput()
    {
        IsMoveKeyInput();
  
        if (_sm.inputHandler.InputGetDownKey(KeyCode.Space, Define.GetKeyType.Down))
        {
            _sm.ChangeState(_sm.jumpingState);
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
        int num = (int)_sm.inputHandler.leftArmPose;
        num++;
        _sm.inputHandler.leftArmPose = ((num <= 3) ? ((BodyPose)num) : BodyPose.Bent);
        int num2 = (int)_sm.inputHandler.rightArmPose;
        num2++;
        _sm.inputHandler.rightArmPose = ((num2 <= 3) ? ((BodyPose)num2) : BodyPose.Bent);
        int num3 = (int)_sm.inputHandler.leftLegPose;
        num3++;
        _sm.inputHandler.leftLegPose = ((num3 <= 3) ? ((BodyPose)num3) : BodyPose.Bent);
        int num4 = (int)_sm.inputHandler.rightLegPose;
        num4++;
        _sm.inputHandler.rightLegPose = ((num4 <= 3) ? ((BodyPose)num4) : BodyPose.Bent);
    }


    public override void UpdatePhysics()
    {

    }
}
