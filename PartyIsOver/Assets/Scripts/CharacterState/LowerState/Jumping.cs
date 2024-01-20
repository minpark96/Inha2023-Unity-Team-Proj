using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Jumping : BodyState
{
    private LowerBodySM sm;

    public Jumping(StateMachine stateMachine):base("JumpingState", stateMachine)
    {
        sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        sm.isGrounded = false;
    }

    public override void UpdateLogic()
    {
        //���� ������
        if (sm.isGrounded)
        {
            sm.ChangeState(sm.idleState);
        }
    }
    public override void GetInput()
    {
        if (IsMoveKeyInput())
            return;
    }

    private bool IsMoveKeyInput()
    {
        KeyCode[] keyCodes = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (sm.inputHandler.InputGetDownKey(keyCodes[i], Define.GetKeyType.Press))
                return true;
        }
        return false;
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
