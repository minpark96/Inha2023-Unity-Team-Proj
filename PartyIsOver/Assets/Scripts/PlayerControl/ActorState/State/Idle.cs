using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{
    float inputSpeed;
    public Idle(MovementSM stateMachine, CameraControl cameraControl) : base("Idel", stateMachine) { }
    public override void Enter()
    {
        base.Enter();
        //TODO : �ӵ� 0���� ����
        inputSpeed = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : Ű �Է� 
        if (Mathf.Abs(inputSpeed) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).movingState);
    }
}