using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{
    float inputspeed;
    public Idle(MovementSM stateMachine) : base("Idel", stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        //TODO : �ӵ� 0���� ����
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : Ű �Է� 
        if (Mathf.Abs(inputspeed) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).movingState);
    }

}
