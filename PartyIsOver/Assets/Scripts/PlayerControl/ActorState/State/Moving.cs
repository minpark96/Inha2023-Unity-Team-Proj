using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : BaseState
{
    private MovementSM _sm;
    float inputspeed;
    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) 
    {
        _sm = (MovementSM)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        //TODO : 속도 0으로 설정
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : 키 입력 추가
        if (Mathf.Abs(inputspeed) < Mathf.Epsilon)
            stateMachine.ChangeState(_sm.idleState);
    }
    public override void UpdatePhysics()
    {
        base.UpdateLogic();
        Vector3 vel = _sm.rigidbody.velocity;
        // TODO : 방향추가
        _sm.rigidbody.velocity = vel;
    }
}
