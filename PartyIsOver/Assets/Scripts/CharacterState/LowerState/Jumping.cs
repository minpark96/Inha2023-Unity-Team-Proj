using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Jumping : BodyState
{
    private LowerBodySM _sm;

    public Jumping(StateMachine stateMachine):base(Define.PlayerState.Jumping, stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void Enter()
    {
        _sm.IsGrounded = false;
    }

    public override void UpdateLogic()
    {
        //상태 나가기
        if (_sm.IsGrounded)
        {
            _sm.ChangeState(_sm.IdleState);
        }
    }
    public override void GetInput()
    {
        IsMoveKeyInput();

        if (_sm.InputHandler.InputCommnadKey(KeyCode.Mouse1, Define.GetKeyType.Down))
        {
            _sm.ChangeState(_sm.DropKickState);
        }
    }

    public override void UpdatePhysics()
    {
    }

    public override void Exit()
    {
    }
}
