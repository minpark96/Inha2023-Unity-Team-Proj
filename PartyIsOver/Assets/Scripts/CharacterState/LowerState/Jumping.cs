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
        //상태 나가기
        if (_sm.isGrounded)
        {
            _sm.ChangeState(_sm.idleState);
        }
    }
    public override void GetInput()
    {
        IsMoveKeyInput();

    }



    //bodyHandler, moveDir(계속 추적해야함,input으로 다른곳에서계산), 그외 잡다한 수치들이 필요
    public override void UpdatePhysics()
    {

    }

    public override void Exit()
    {
    }
}
