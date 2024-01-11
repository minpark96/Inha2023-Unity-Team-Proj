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
        //TODO : 속도 0으로 설정
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : 키 입력 
        if (Mathf.Abs(inputspeed) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).movingState);
    }

}
