using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Jumping : BodyState
{
    private LowerBodySM _sm;

    public Jumping(StateMachine stateMachine):base("JumpingState", stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        _sm.isGrounded = false;
    }

    public override void UpdateLogic()
    {
        //���� ������
        if (_sm.isGrounded)
        {
            _sm.ChangeState(_sm.idleState);
        }
    }
    public override void GetInput()
    {
        IsMoveKeyInput();

    }



    //bodyHandler, moveDir(��� �����ؾ���,input���� �ٸ����������), �׿� ����� ��ġ���� �ʿ�
    public override void UpdatePhysics()
    {

    }

    public override void Exit()
    {
    }
}
