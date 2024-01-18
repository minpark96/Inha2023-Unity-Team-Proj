using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{

    private float horizontalInput;
    private float verticalInput;
    public Idle(MovementSM stateMachine) : base("Idel", stateMachine) 
    {
    }
    public override void Enter()
    {
        base.Enter();
        //TODO : 속도 0으로 설정
        horizontalInput = 0f;
        verticalInput = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //TODO : 키 입력 
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon || Mathf.Abs(verticalInput) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).MovingState);
    }
}