using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Jumping : BaseState
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
        if (IsMoveKeyInput())
            InvokeReserveCommand(COMMAND_KEY.Move);

        if (InputCommand(COMMAND_KEY.RightBtn, KeyType.Down))
        {
            InvokeReserveCommand(COMMAND_KEY.RightBtn);
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
