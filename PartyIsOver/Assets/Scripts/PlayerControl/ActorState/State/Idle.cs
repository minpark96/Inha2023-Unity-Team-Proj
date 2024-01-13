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
        //TODO : 속도 0으로 설정
        inputSpeed = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : 키 입력 
        if (Mathf.Abs(inputSpeed) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).movingState);
    }
}