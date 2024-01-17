using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Jumping : BodyState
{
    protected LowerBodySM sm;

    public Jumping(StateMachine stateMachine):base("JumpingState", stateMachine)
    {
        sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {

    }

    public override void UpdateLogic()
    {
        //���� ������
        if (sm.isGrounded)
        {
            sm.ChangeState(sm.idleState);
        }
    }

    //bodyHandler, moveDir(��� �����ؾ���,input���� �ٸ����������), �׿� ����� ��ġ���� �ʿ�
    public override void UpdatePhysics()
    {
       

        //if (_hips.velocity.magnitude > MaxSpeed)
        //    _hips.velocity = _hips.velocity.normalized * MaxSpeed;
    }

    public override void Exit()
    {
    }
}
