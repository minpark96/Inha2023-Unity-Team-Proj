using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
// 점프 상태
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
        //밟을 수 있는 무언가에 충돌하면 상태 나가기
        if (_sm.IsGrounded)
        {
            _sm.ChangeState(_sm.IdleState);
        }
    }
    public override void GetInput()
    {
        // 점프 도중에 약간의 이동기능
        if (IsMoveKeyInput())
            InvokeReserveCommand(COMMAND_KEY.Move);
        
        // 드롭킥으로 상태 변경
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
