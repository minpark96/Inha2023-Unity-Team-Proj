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
        //TODO : �ӵ� 0���� ����
        horizontalInput = 0f;
        verticalInput = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //TODO : Ű �Է� 
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon || Mathf.Abs(verticalInput) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).MovingState);
    }
}