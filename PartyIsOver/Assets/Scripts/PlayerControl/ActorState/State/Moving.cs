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
        //TODO : �ӵ� 0���� ����
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : Ű �Է� �߰�
        if (Mathf.Abs(inputspeed) < Mathf.Epsilon)
            stateMachine.ChangeState(_sm.idleState);
    }
    public override void UpdatePhysics()
    {
        base.UpdateLogic();
        Vector3 vel = _sm.rigidbody.velocity;
        // TODO : �����߰�
        _sm.rigidbody.velocity = vel;
    }
}
